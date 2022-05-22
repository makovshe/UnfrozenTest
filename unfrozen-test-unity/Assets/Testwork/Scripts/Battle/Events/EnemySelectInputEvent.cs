using Testwork.Events;
using Testwork.Scripts.Characters;

namespace Testwork.Scripts.Battle.Events
{
    public struct EnemySelectInputEvent : IGameEvent
    {
        public Character Character;
    }
}