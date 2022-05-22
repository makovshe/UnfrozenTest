using Testwork.Events;

namespace Testwork.Scripts.Characters.Events
{
    public struct CharacterPointerClickEvent : IGameEvent
    {
        public Character Character;
    }
}