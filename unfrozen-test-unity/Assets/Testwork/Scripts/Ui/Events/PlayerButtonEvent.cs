using Testwork.Events;
using UnityEngine;

namespace Testwork.Ui.Events
{
    public struct PlayerButtonEvent : IGameEvent
    {
        public GameObject Source;
    }
}