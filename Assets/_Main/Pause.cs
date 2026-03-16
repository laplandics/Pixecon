using System;
using UnityEngine;

public class Pause : IDisposable
{
    public Pause()
    {
        Core.Event.Subscribe<PauseGameEvent>(PauseGame);
        Core.Event.Subscribe<ResumeGameEvent>(ResumeGame);
    }

    private void PauseGame(PauseGameEvent _)
    { Time.timeScale = 0; }

    private void ResumeGame(ResumeGameEvent _)
    { Time.timeScale = 1; }
    
    public void Dispose()
    {
        Core.Event.UnSubscribe<PauseGameEvent>(PauseGame);
        Core.Event.UnSubscribe<ResumeGameEvent>(ResumeGame);
    }
}