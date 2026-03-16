using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public static class Coroutine
    {
        private static Dictionary<Func<IEnumerator>, UnityEngine.Coroutine> _routines;
        
        public static void Reset()
        { _routines = new Dictionary<Func<IEnumerator>, UnityEngine.Coroutine>(); }
        
        
        public static UnityEngine.Coroutine Start(Func<IEnumerator> function)
        {
            Clear();
            if (function == null) return null;
            var routine = function();
            var coroutine = Empty.Get.StartCoroutine(routine);
            _routines[function] = coroutine;
            return null;
        }

        private static void Clear()
        {
            if (_routines.Count != 0)
            {
                var nullRoutines = new List<Func<IEnumerator>>();
                foreach (var pair in _routines)
                { if (pair.Value == null) nullRoutines.Add(pair.Key); }
                foreach (var nullRoutine in nullRoutines) { _routines.Remove(nullRoutine); }
            }
            else { _routines.Clear(); }
        }

        public static void Stop(Func<IEnumerator> function)
        {
            if (!_routines.TryGetValue(function, out var coroutine)) return;
            if (coroutine == null) {_routines.Remove(function); return; }
            Empty.Get.StopCoroutine(coroutine);
            _routines.Remove(function);
        }
    }
}