using System.IO;
using UnityEngine;

namespace Core
{
    public static class Data
    {
        public static global::Data Get { get; private set; }

        private static string GetPath => $"{Path.Combine(Application.persistentDataPath, nameof(Data))}.json";

        public static void Reset() { Get = new global::Data(); }

        public static void Save()
        {
            var json = JsonUtility.ToJson(Get, true);
            File.WriteAllText(GetPath, json);
        }
        
         public static void Load()
        {
            Get = new global::Data();
            if (!File.Exists(GetPath)) return;
            var json = File.ReadAllText(GetPath);
            var data = JsonUtility.FromJson<global::Data>(json);
            Get = data;
        }
    }
}