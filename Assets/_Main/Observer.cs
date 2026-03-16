using System;
using System.Collections.Generic;

namespace Core
{
    public static class Observer
    {
        private static readonly Dictionary<Type, object> RegisteredClasses = new();

        public static void Reset() { RegisteredClasses.Clear(); }
        
        public static void Register<T>(object o)
        { var type = typeof(T); RegisteredClasses.TryAdd(type, o); }

        public static T Get<T>()
        { if (RegisteredClasses.TryGetValue(typeof(T), out var o)) return o is not T registered ? default : registered;
            return default; }

        public static void Unregister<T>()
        { RegisteredClasses.Remove(typeof(T)); }
    }
}