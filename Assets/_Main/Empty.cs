using UnityEngine;

namespace Core
{
    public static class Empty
    {
        public static EmptyObject Get { get; private set; }
        
        public static void Instantiate()
        {
            if (Get != null)
            { Debug.LogWarning("Empty already instantiated"); Object.Destroy(Get.gameObject); }
            Get = new GameObject("Empty").AddComponent<EmptyObject>();
            Object.DontDestroyOnLoad(Get.gameObject);
        }
    }

    public class EmptyObject : MonoBehaviour {}
}