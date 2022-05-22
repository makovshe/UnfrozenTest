using Testwork.Events;
using Testwork.Scripts.Characters;

namespace Testwork.Scripts.Battle.Events
{
    public struct CharacterAttackEvent : IGameEvent
    {
        public Character Character;
    }
}