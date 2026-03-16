using System;
using System.Collections.Generic;

namespace Core
{
    public static class Event
    {
        private static Dictionary<Type, Delegate> _subscribers;
        
        public static void Reset() { _subscribers = new Dictionary<Type, Delegate>(); }

        public static void Subscribe<T>(Action<T> handler) where T : global::Event
        {
            var type = typeof(T);
            if (!_subscribers.TryAdd(type, handler)) _subscribers[type] = (Action<T>)_subscribers[type] + handler;
        }

        public static void UnSubscribe<T>(Action<T> handler) where T : global::Event
        {
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var existing)) return;
            existing = (Action<T>)existing - handler;
            if (existing == null) _subscribers.Remove(type);
            else _subscribers[type] = existing;
        }

        public static void Invoke<T>(T eventData) where T : global::Event
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var del)) (del as Action<T>)?.Invoke(eventData);
        }
    }
}