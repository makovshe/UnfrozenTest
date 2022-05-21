using System;
using UnityEngine;

namespace Content.Scripts
{
    public sealed class BattleManager : MonoBehaviour
    {
        [SerializeField] private SceneCharacterPivots _scenePivots;
        
        //TODO Character fields must not be serializable. Arrays must be initialized from outside
        [SerializeField] private Character[] _leftTeamPrefabs = new Character[0];
        [SerializeField] private Character[] _rightTeamPrefabs = new Character[0];

        private Character[] _leftCharactersInstances;
        private Character[] _rightCharactersInstances;
        
        private void Awake()
        {
            _leftCharactersInstances = SpawnCharacters(_scenePivots.LeftTeam, _leftTeamPrefabs, false);
            _rightCharactersInstances = SpawnCharacters(_scenePivots.RightTeam, _rightTeamPrefabs, true);
        }

        private Character[] SpawnCharacters(Transform[] pivots, Character[] characters, bool isRight)
        {
            if (pivots.Length != characters.Length)
            {
                Debug.LogError($"SceneCharacterPivots length not equal {(isRight?"right":"left")} characters length");
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
                result[i] = character;
                character.PlayIdle();
            }
            return result;
        }
    }
}