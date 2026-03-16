using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformQueue : IDisposable
{
    private bool _queueInProgress;
    private Queue<GameBox> _boxQueue = new();
    
    public PerformQueue() {Core.Coroutine.Start(Queue); }

    public bool TryAddBox(GameBox box)
    {
        if (_boxQueue.Contains(box)) return false;
        if (!box.CanBeInQueue) return false;
        box.CanBeInQueue = false;
        _boxQueue.Enqueue(box);
        box.GetReady();
        _queueInProgress = true;
        return true;
    }
    
    private IEnumerator Queue()
    {
        yield return new WaitUntil(() => _queueInProgress);
        while (_queueInProgress)
        {
            yield return null;
            yield return _boxQueue.Peek().Perform();
            _boxQueue.Dequeue();
            
            if (_boxQueue.Count != 0) continue;
            yield return new WaitForSeconds(0.5f);
            
            if (_boxQueue.Count != 0) continue;
            _queueInProgress = false;
            Core.Event.Invoke(new GameOverEvent());
        }
    }

    public void Dispose() { Core.Coroutine.Stop(Queue); }
}