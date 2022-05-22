using Testwork.Events;
using Testwork.Scripts.Battle.Events;
using Testwork.Scripts.Characters;
using Testwork.Scripts.Characters.Events;
using Testwork.Ui.Events;
using UnityEngine;

namespace Testwork.Scripts.Battle
{
    public sealed class CharacterSelectionManager : MonoBehaviour, IGameEventListener<CharacterPointerEnterEvent>,
        IGameEventListener<CharacterPointerExitEvent>, IGameEventListener<CharacterPointerClickEvent>, IGameEventListener<CharacterTurnEvent>,
         IGameEventListener<CharacterAttackEvent>, IGameEventListener<AttackInputEvent>
    {
        [SerializeField] private GameObject CharacterSelectedPrefab;
        [SerializeField] private GameObject EnemySelectedPrefab;

        private GameEventsManager _eventsManager;
        private GameObject _characterSelectedObject;
        private GameObject _enemySelectedObject;
        private bool _enemySelectionAvailable;

        private void Awake()
        {
            _eventsManager = FindObjectOfType<GameEventsManager>();
            _eventsManager.AddListener<CharacterTurnEvent>(this);
            _eventsManager.AddListener<CharacterAttackEvent>(this);
            _eventsManager.AddListener<CharacterPointerEnterEvent>(this);
            _eventsManager.AddListener<CharacterPointerExitEvent>(this);
            _eventsManager.AddListener<CharacterPointerClickEvent>(this);
            _eventsManager.AddListener<AttackInputEvent>(this);
            
            _characterSelectedObject = Instantiate(CharacterSelectedPrefab);
            _characterSelectedObject.SetActive(false);
            _enemySelectedObject = Instantiate(EnemySelectedPrefab);
            _enemySelectedObject.SetActive(false);
        }

        public void OnEvent(CharacterPointerEnterEvent gameEvent)
        {
            if (!_enemySelectionAvailable || gameEvent.Character.IsControlledByPlayer)
            {
                return;
            }
            SelectCharacter(_enemySelectedObject, gameEvent.Character);
        }

        public void OnEvent(CharacterPointerExitEvent gameEvent)
        {
            if (_enemySelectionAvailable)
            {
                DisableEnemyObject();
            }
        }
        
        public void OnEvent(CharacterPointerClickEvent gameEvent)
        {
            if (!_enemySelectionAvailable || gameEvent.Character.IsControlledByPlayer)
            {
                return;
            }
            _eventsManager.SendEvent(new EnemySelectInputEvent {Character = gameEvent.Character});
        }

        public void OnEvent(CharacterTurnEvent gameEvent)
        {
            DisableEnemyObject();
            SelectCharacter(_characterSelectedObject, gameEvent.Character);
        }

        public void OnEvent(CharacterAttackEvent gameEvent)
        {
            _enemySelectionAvailable = false;
            _characterSelectedObject.SetActive(false);
            DisableEnemyObject();
        }
        
        public void OnEvent(AttackInputEvent gameEvent)
        {
            _enemySelectionAvailable = true;
        }
        
        private void SelectCharacter(GameObject selectObject, Character character)
        {
            if (!selectObject.activeSelf)
            {
                selectObject.SetActive(true);
            }
            selectObject.transform.position = character.transform.position;
        }

        private void DisableEnemyObject()
        {
            if (_enemySelectedObject.activeSelf)
            {
                _enemySelectedObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _eventsManager.RemoveListener<CharacterTurnEvent>(this);
            _eventsManager.RemoveListener<CharacterAttackEvent>(this);
            _eventsManager.RemoveListener<CharacterPointerEnterEvent>(this);
            _eventsManager.RemoveListener<CharacterPointerExitEvent>(this);
            _eventsManager.RemoveListener<CharacterPointerClickEvent>(this);
            _eventsManager.RemoveListener<AttackInputEvent>(this);
        }
    }
}