using Testwork.Events;

namespace Testwork.Scripts.Characters.Events
{
    public struct CharacterPointerEnterEvent : IGameEvent
    {
        public Character Character;
    }
}