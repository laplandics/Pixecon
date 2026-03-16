using System.Collections;
using UnityEngine.SceneManagement;

namespace Core
{
    public static class Scene
    {
        public const string BOOT = "Boot";
        public const string MENU = "Menu(old)";
        public const string VOCAB = "Vocab";
        public const string GAME = "Game";
        
        public static IEnumerator Load(string sceneName) {
            yield return SceneManager.LoadSceneAsync(0); yield return null;
            yield return SceneManager.LoadSceneAsync(sceneName); yield return null; }
    }
}