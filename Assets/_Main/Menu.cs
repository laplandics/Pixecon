using UnityEngine;

public class Menu : MonoBehaviour
{
    public static void Begin() { new GameObject("Menu").AddComponent<Menu>().StartMenu(); }
    public static void End() { FindFirstObjectByType<Menu>()?.StopMenu();}
    
    
    private void StartMenu()
    {
        MenuUi.Load();
        
    }

    private void StopMenu()
    {
        
    }
}