using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Field : MonoBehaviour
{
    public bool queueInProgress;
    private Inputs _inputs;
    private PerformQueue _queue;
    private float _currentDelay;
    private List<WeightedValue<Type>> _effects = new();
    private List<WeightedValue<int>> _effectsWeighted = new();
    private int _maxEffectsCount;
    private bool _shouldUpdateField;
    
    public Grid Grid { get; private set; }
    public Dictionary<Vector2Int, GameBox> Boxes { get; private set; }

    public IEnumerator Init()
    {
        Core.Observer.Register<Field>(this);
        InitializeBoxEffects();
        yield return BuildBoxes();
        yield return SetBoxEffects();
        
        _inputs = Core.Input.Get;
        _queue = new PerformQueue();
        _currentDelay = Settings.Get.firstPerformDelay;
        
        Core.Event.Subscribe<GameSceneLoadedEvent>(LaunchField);
        Core.Event.Subscribe<GameBoxDestroyedEvent>(UpdateField);
    }

    private void InitializeBoxEffects()
    {
        var effectTypeWeights = Settings.Get.effectsTypeWeights;
        foreach (var etw in effectTypeWeights)
        {
            var effectType = Type.GetType(etw.effectType.ToString());
            if (effectType == null)
            { Debug.LogError($"Box Effect {etw.effectType} not found"); continue; }
            var weightedValue = new WeightedValue<Type> {value = effectType, weight = etw.weight};
            _effects.Add(weightedValue);
        }

        _effectsWeighted = new List<WeightedValue<int>>();
        var effectsCountWeights = Settings.Get.effectsCountWeights;
        foreach (var effectCountWeight in effectsCountWeights)
        {
            var effectCount = effectCountWeight.effectsCount;
            var probability = effectCountWeight.weight;
            var weightedValue = new WeightedValue<int> { value = effectCount, weight = probability };
            _effectsWeighted.Add(weightedValue);
        }
        _maxEffectsCount = _effectsWeighted.Count;
    }

    private IEnumerator BuildBoxes()
    {
        Grid = new Grid();
        Boxes = new Dictionary<Vector2Int, GameBox>();
        var wait = new WaitForSeconds(0.01f);
        foreach (var position in Grid.GridPositions)
        {
            var worldPos = Grid.GridToWorld(position);
            var boxPrefab = Resources.Load<GameObject>("Game/GameBox");
            var boxObj = Instantiate(boxPrefab, worldPos, Quaternion.identity, transform);
            var box = boxObj.AddComponent<GameBox>();
            box.Initialize();
            InitializeBox(box, position);
            yield return wait;
        }
        Resources.UnloadUnusedAssets();
    }
    
    private void InitializeBox(GameBox box, Vector2Int position)
    {
        box.gameObject.SetActive(true);
        box.gameObject.name = "GameBox";
        box.Born(position);
        Boxes.Add(position, box);
    }

    private IEnumerator SetBoxEffects()
    {
        var wait = new WaitForSeconds(0.01f);
        foreach (var box in Boxes.Values)
        { SetRandomBoxEffects(box); yield return wait; }
    }
    
    private void SetRandomBoxEffects(GameBox box)
    {
        var boxEffectCount = WeightedRandom.GetRandom(_effectsWeighted); 
        var boxEffects = new BoxEffect[boxEffectCount]; 
        
        for (var i = 0; i < boxEffects.Length; i++)
        {
            var newEffectType = WeightedRandom.GetRandom(_effects);
            var newEffect = (BoxEffect)Activator.CreateInstance(newEffectType);
            newEffect.name = newEffectType.Name;
            boxEffects[i] = newEffect;
        }
        
        var value = 1f / _maxEffectsCount;
        var colorValue = value * boxEffectCount;
        box.ChangeColor(new Color(colorValue, colorValue, colorValue));
        
        box.SetEffects(boxEffects);
    }
    
    private void LaunchField(GameSceneLoadedEvent _)
    {
        Core.Event.UnSubscribe<GameSceneLoadedEvent>(LaunchField);
        _shouldUpdateField = true;
        _inputs.Game.Pos.Enable();
        _inputs.Game.Tap.Enable();
        _inputs.Game.Tap.canceled += OnTap;
        Core.Coroutine.Start(CalculateDelay);
    }
    
    private void OnTap(InputAction.CallbackContext _)
    {
        var screenPos = _inputs.Game.Pos.ReadValue<Vector2>();
        var worldPos = Core.Cam.Get.ScreenToWorldPoint(screenPos);
        var ray = new Ray(worldPos, Core.Cam.Get.transform.forward);
        if (!Physics.Raycast(ray, out var hit)) return;
        if (!hit.collider.gameObject.TryGetComponent<GameBox>(out var box)) return;
        box.PerformDelay = _currentDelay;
        if(!_queue.TryAddBox(box)) return;
        queueInProgress = true;
    }
    
    private IEnumerator CalculateDelay()
    {
        var waitForQueue = new WaitUntil(() => queueInProgress);
        var waitForInterval = new WaitForSeconds(Settings.Get.delayDecreaseInterval);
        var decreaseRate = Settings.Get.delayDecreaseRate;
        _currentDelay = Settings.Get.baseDelay;
        
        while (true)
        {
            yield return waitForQueue;
            _currentDelay *= decreaseRate;
            yield return waitForInterval;
        }
    }

    private void UpdateField(GameBoxDestroyedEvent eventData)
    {
        if (!_shouldUpdateField) return;
        var position = eventData.DestroyedGb.PositionOnGrid;
        Boxes.Remove(position);
        
        for (var y = 0; y < Grid.RowsCount; y++)
        {
            var pos = new Vector2Int(position.x, y);
            if (Boxes.ContainsKey(pos)) continue;
            for (var above = y + 1; above < Grid.RowsCount; above++)
            {
                var posAbove = new Vector2Int(position.x, above);
                if (!Boxes.Remove(posAbove, out var box)) continue;
                Boxes.Add(pos, box);
                box.PositionOnGrid = pos;
                box.UpdateFallTarget(Grid.GridToWorld(pos), Grid.GridToWorld(position));
                break;
            }
        }
        var newPos = new Vector2Int(position.x, Grid.RowsCount - 1);
        eventData.DestroyedGb.transform.localPosition = Grid.GridToWorld(newPos);
        eventData.DestroyedGb.gameObject.name = "recreated box";
        InitializeBox(eventData.DestroyedGb, newPos);
        SetRandomBoxEffects(eventData.DestroyedGb);
    }

    public IEnumerator ClearField()
    {
        _shouldUpdateField = false;
        Core.Event.Invoke(new PauseGameEvent());
        var wait = new WaitForSecondsRealtime(0.01f);
        queueInProgress = false;
        _queue.Dispose();
        foreach (var gridPosition in Grid.GridPositions)
        { Boxes[gridPosition].ForceDie(); yield return wait; }
        Boxes.Clear();
        _inputs.Game.Pos.Disable();
        _inputs.Game.Tap.Disable();
        Core.Event.Invoke(new ResumeGameEvent());
    }
    
    public void DeInit()
    {
        Core.Observer.Unregister<Field>();
        _queue.Dispose();
        Core.Coroutine.Stop(CalculateDelay);
        Core.Event.UnSubscribe<GameSceneLoadedEvent>(LaunchField);
        Core.Event.UnSubscribe<GameBoxDestroyedEvent>(UpdateField);
        _inputs.Game.Tap.Disable();
        _inputs.Game.Pos.Disable();
        _inputs.Game.Tap.canceled -= OnTap;
    }
}