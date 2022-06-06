using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Testwork.Events;
using Testwork.Scripts.Battle.Events;
using Testwork.Scripts.Characters;
using Testwork.Ui.Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Testwork.Scripts.Battle
{
    //TODO move characters logic to separate class, this class only for battle state
    public sealed class BattleManager : MonoBehaviour, IGameEventListener<EnemySelectInputEvent>,
        IGameEventListener<AttackHitAnimationEvent>, IGameEventListener<SkipTurnInputEvent>
    {
        private enum BattleState
        {
            PlayerTurn,
            PrepareAttack,
            Attack,
            AfterAttack,
            EnemyTurn
        }

        [SerializeField] private SceneCharacterPivots _scenePivots;

        [SerializeField] private Character[] _leftTeamPrefabs = new Character[0];
        [SerializeField] private Character[] _rightTeamPrefabs = new Character[0];

        [SerializeField] private float _prepareToAttackTime = 1f;
        [SerializeField] private float _afterAttackDelay = 0.5f;
        [SerializeField] private float _botTurnDelay = 1f;

        private GameEventsManager _eventsManager;
        private Character[] _leftCharactersInstances;
        private Character[] _rightCharactersInstances;
        private Character[] _battleQueue;
        private BattleState _battleState;
        private Character _selectedEnemy;
        private Character _selectedCharacter;
        private int _nextCharacterIndex;
        private bool _isDamageProcess;
        private IEnumerator _damageCoroutine;

        private void Awake()
        {
            _eventsManager = FindObjectOfType<GameEventsManager>();
            _eventsManager.AddListener<EnemySelectInputEvent>(this);
            _eventsManager.AddListener<AttackHitAnimationEvent>(this);
            _eventsManager.AddListener<SkipTurnInputEvent>(this);
        }

        private void Start()
        {
            _leftCharactersInstances = SpawnCharacters(_scenePivots.LeftTeam, _leftTeamPrefabs, false);
            _rightCharactersInstances = SpawnCharacters(_scenePivots.RightTeam, _rightTeamPrefabs, true);
            var battleQueue = new List<Character>();
            battleQueue.AddRange(_leftCharactersInstances);
            battleQueue.AddRange(_rightCharactersInstances);
            _battleQueue = RandomizeOrder(battleQueue.ToArray());
            SelectNewCharacter();
        }

        private Character[] SpawnCharacters(Transform[] pivots, Character[] characters, bool isRight)
        {
            if (pivots.Length != characters.Length)
            {
                Debug.LogError(
                    $"SceneCharacterPivots length not equal {(isRight ? "right" : "left")} characters length");
                return new Character[0];
            }

            var result = new Character[pivots.Length];
            for (var i = 0; i < pivots.Length; i++)
            {
                var character = Instantiate(characters[i], pivots[i].position, Quaternion.identity);
                if (isRight)
                {
                    character.FlipSkeleton();
                }
                else
                {
                    // TODO bad decision to assume that the leftCharacters are always controlled by the player
                    character.SetControlledByPlayer(true);
                }

                character.PlayIdle();
                result[i] = character;
            }

            return result;
        }

        private void SetCharacterTurn(Character character)
        {
            _selectedCharacter = character;
            _eventsManager.SendEvent(new CharacterTurnEvent {Character = character});
        }

        public void OnEvent(EnemySelectInputEvent gameEvent)
        {
            if (_battleState != BattleState.PlayerTurn)
            {
                return;
            }
            _selectedEnemy = gameEvent.Character;
            StartCoroutine(StartPrepareAttack());
        }

        private IEnumerator StartPrepareAttack()
        {
            _battleState = BattleState.PrepareAttack;
            _eventsManager.SendEvent(new CharacterAttackEvent {Character = _selectedCharacter});

            _selectedCharacter.SetAttackOrderInLayer();
            _selectedEnemy.SetAttackOrderInLayer();
            var selectedCharacterPosition = _selectedCharacter.transform.position;
            var enemyPosition = _selectedEnemy.transform.position;
            var selectedCharacterPivotPosition = GetAttackPivotPosition(_selectedCharacter);
            var enemyPivotPosition = GetAttackPivotPosition(_selectedEnemy);
            var time = 0f;
            while (time < _prepareToAttackTime)
            {
                Lerp(_selectedCharacter, selectedCharacterPosition, selectedCharacterPivotPosition, time, _prepareToAttackTime);
                Lerp(_selectedEnemy, enemyPosition, enemyPivotPosition, time, _prepareToAttackTime);
                time += Time.deltaTime;
                yield return null;
            }

            _selectedCharacter.transform.position = selectedCharacterPivotPosition;
            _selectedEnemy.transform.position = enemyPivotPosition;
            _battleState = BattleState.Attack;
            yield return new WaitForSeconds(_selectedCharacter.PlayAttack());
            
            _selectedCharacter.PlayIdle();
            yield return new WaitForSeconds(_afterAttackDelay);
            _battleState = BattleState.AfterAttack;
            _eventsManager.SendEvent(new AfterAttackEvent());

            time = 0;
            while (time < _prepareToAttackTime)
            {
                Lerp(_selectedCharacter,selectedCharacterPivotPosition, selectedCharacterPosition, time, _prepareToAttackTime);
                Lerp(_selectedEnemy,enemyPivotPosition, enemyPosition, time, _prepareToAttackTime);
                time += Time.deltaTime;
                yield return null;
            }
            _selectedCharacter.transform.position = selectedCharacterPosition;
            _selectedEnemy.transform.position = enemyPosition;
            _selectedCharacter.SetDefaultOrderInLayer();
            _selectedEnemy.SetDefaultOrderInLayer();
            SelectNewCharacter();
        }

        private Vector3 GetAttackPivotPosition(Character character)
        {
            return character.IsLeft ? _scenePivots.LeftAttackPivot.position : _scenePivots.RightAttackPivot.position;
        }

        private void Lerp(Character character, Vector3 source, Vector3 target, float currentTime, float totalTime)
        {
            character.transform.position = Vector3.Lerp(source, target, currentTime / totalTime);
        }

        public void OnEvent(AttackHitAnimationEvent gameEvent)
        {
            if (_isDamageProcess)
            {
                StopCoroutine(_damageCoroutine);
            }

            _damageCoroutine = StartDamage();
            StartCoroutine(_damageCoroutine);
        }

        private IEnumerator StartDamage()
        {
            _isDamageProcess = true;
            yield return new WaitForSeconds(_selectedEnemy.PlayDamaged());
            _isDamageProcess = false;
            _selectedEnemy.PlayIdle();
        }

        
        public void OnEvent(SkipTurnInputEvent gameEvent)
        {
            SelectNewCharacter();
        }
        
        private void SelectNewCharacter()
        {
            if (_nextCharacterIndex >= _battleQueue.Length)
            {
                _battleQueue = RandomizeOrder(_battleQueue);
                _nextCharacterIndex = 0;
            }

            var character = _battleQueue[_nextCharacterIndex];
            _battleState = character.IsControlledByPlayer ? BattleState.PlayerTurn : BattleState.EnemyTurn;
            SetCharacterTurn(character);
            _nextCharacterIndex++;

            if (_battleState == BattleState.EnemyTurn)
            {
                StartCoroutine(StartBotTurn());
            }
        }

        private IEnumerator StartBotTurn()
        {
            yield return new WaitForSeconds(_botTurnDelay);
            _selectedEnemy = _leftCharactersInstances[Random.Range(0, _leftCharactersInstances.Length)];
            StartCoroutine(StartPrepareAttack());
        }

        private void OnDestroy()
        {
            _eventsManager.RemoveListener<EnemySelectInputEvent>(this);
            _eventsManager.RemoveListener<AttackHitAnimationEvent>(this);
            _eventsManager.RemoveListener<SkipTurnInputEvent>(this);
        }
        
        //TODO move to utilities
        private static T[] RandomizeOrder<T>(IEnumerable<T> array)
        {
            const int minRange = -1000;
            const int maxRange = 1000;
            return array.OrderBy(m => UnityEngine.Random.Range(minRange, maxRange)).ToArray();
        }
    }
}