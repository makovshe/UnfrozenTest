using Testwork.Events;

namespace Testwork.Scripts.Characters.Events
{
    public struct CharacterPointerExitEvent : IGameEvent
    {
        public Character Character;
    }
}