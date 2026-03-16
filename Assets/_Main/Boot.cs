using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class Boot
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            Empty.Instantiate();
            Ui.Instantiate();
            
            Observer.Reset();
            Coroutine.Reset();
            Event.Reset();
            Input.Reset();
            Data.Reset();
            
            Event.UnSubscribe<LoadMenuEvent>(BootMenu);
            Event.UnSubscribe<LoadGameEvent>(BootGame);
            Event.Subscribe<LoadMenuEvent>(BootMenu);
            Event.Subscribe<LoadGameEvent>(BootGame);

#if UNITY_EDITOR
            
           // UNITY EDITOR ONLY 
            var scene = SceneManager.GetActiveScene();
            var sceneName = scene.name;
            if (sceneName != Scene.BOOT) 
            { Debug.LogWarning("To start game enter scene \"Boot\" "); return; }
            // UNITY EDITOR ONLY
            
#endif
            
            Coroutine.Start(LoadMenu);
        }

        private static void BootMenu(LoadMenuEvent eventData)
        { Game.End(); Coroutine.Start(LoadMenu); }

        private static void BootGame(LoadGameEvent eventData)
        { Menu.End(); Coroutine.Start(LoadGame); }
        
        private static IEnumerator LoadMenu()
        {
            yield return Scene.Load(Scene.MENU);
            Cam.Instantiate();
            Menu.Begin();
        }

        private static IEnumerator LoadGame()
        {
            yield return Scene.Load(Scene.GAME);
            Cam.Instantiate();
            Game.Begin();
        }
    }
}