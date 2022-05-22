using Testwork.Events;
using Testwork.Scripts.Characters;

namespace Testwork.Scripts.Battle.Events
{
    public struct CharacterTurnEvent : IGameEvent
    {
        public Character Character;
    }
}