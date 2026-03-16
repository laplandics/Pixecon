using UnityEngine;

namespace Core
{
    public static class Cam
    {
        public static Camera Get { get; private set; }
        
        public static void Instantiate()
        {
            if (Get != null)
            { Debug.LogWarning("Camera already instantiated"); Object.Destroy(Get); }
            var camPrefab = Resources.Load<GameObject>("Common/Cam");
            var cam = Object.Instantiate(camPrefab);
            cam.transform.position = new Vector3(0, 0, -10);
            cam.name = "Cam";
            Get = cam.GetComponent<Camera>();
            Get.tag = "MainCamera";
            Resources.UnloadUnusedAssets();
        }

        public static void Destroy()
        {
            if (Get == null)
            { Debug.LogWarning("Camera not instantiated"); return; }
            Object.Destroy(Get.gameObject);
        }
    }
}