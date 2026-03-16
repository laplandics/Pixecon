using UnityEngine;

namespace Core
{
    public static class Ui
    {
        public static Canvas Get { get; private set; }
        
        public static void Instantiate()
        {
            if (Get != null)
            { Debug.LogWarning("Ui already instantiated"); Object.Destroy(Get.gameObject); }
            var uiPrefab = Resources.Load<GameObject>("Common/UI");
            var ui = Object.Instantiate(uiPrefab);
            ui.name = "UI";
            Object.DontDestroyOnLoad(ui);
            Get = ui.GetComponentInChildren<Canvas>();
            Resources.UnloadUnusedAssets();
        }
    }
}