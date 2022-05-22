using System;
using System.Collections.Generic;
using UnityEngine;

namespace Testwork.Events
{
    //Move to service with DI resolve
    public class GameEventsManager : MonoBehaviour
    {
        private readonly Dictionary<Type, HashSet<IGameEventListener>> _listeners = new Dictionary<Type,  HashSet<IGameEventListener>>();
        private readonly Dictionary<Type, List<IGameEventListener>> _buffer = new Dictionary<Type, List<IGameEventListener>>();
        
        public void AddListener<T>(IGameEventListener<T> listener) where T : IGameEvent
        {
            if (!_listeners.TryGetValue(typeof(T), out var listeners))
            {
                listeners = new HashSet<IGameEventListener>();
                _listeners[typeof(T)] = listeners;
            }
            listeners.Add(listener);
        }

        public void SendEvent<T>(T gameEvent) where T : IGameEvent
        {
            if (!_listeners.TryGetValue(typeof(T), out var listeners) || listeners.Count == 0)
            {
                return;
            }

            if (!_buffer.TryGetValue(typeof(T), out var buffer))
            {
                buffer = new List<IGameEventListener>();
                _buffer[typeof(T)] = buffer;
            }

            buffer.AddRange(listeners);
            for (var i = 0; i < buffer.Count; i++)
            {
                ((IGameEventListener<T>) buffer[i]).OnEvent(gameEvent);
            }

            buffer.Clear();
        }

        public void RemoveListener<T>(IGameEventListener listener)
        {
            if (_listeners.TryGetValue(typeof(T), out var listeners))
            {
                listeners.Remove(listener);
            }
        }
    }
}