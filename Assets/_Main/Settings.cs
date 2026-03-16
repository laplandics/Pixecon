using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Data/Settings")]
public class Settings : ScriptableObject
{
    private static Settings _instance;

    public static Settings Get
    { get { _instance ??= Resources.Load<Settings>("Data/Settings"); return _instance; } }
    
    
    public static readonly int GameBoxCol = Shader.PropertyToID("_Color");
    public enum GBEffects
    { SetLetter, SetScore, SetRandomLetter, SetCorrectLetter, Explode, RandomizeField, PreformRandomEffect, SetChosenLetter }
    public enum GBEffectsLayouts
    { OneEffect, TwoEffects, ThreeEffects, FourEffects, }
    [Serializable] public class EffectsCountWeight { public int effectsCount; public int weight; }
    [Serializable] public class EffectsTypeSettings { public GBEffects effectType; public int weight; }
    [Serializable] public class EffectsLayoutPreset
    { public GBEffectsLayouts layout; public Vector2[] tilings; public Vector2[] offsets;}
    
    [Header("Game Box Settings")]
    public EffectsCountWeight[] effectsCountWeights;
    public EffectsTypeSettings[] effectsTypeWeights;
    public EffectsLayoutPreset[] effectsLayoutPresets;
    
    public float firstPerformDelay = 1.5f;
    public float baseDelay = 1.1f;
    public float delayDecreaseInterval = 1.0f;
    public float delayDecreaseRate = 0.999f;

    public float fallAnimationSpeed = 8f;
    public float fallAnimationMaxSpeed = 9.3f;
    public float fallAnimationMinSpeed = 3.2f;

    public float deathAnimationSpeed = 4.8f;

    public float lifeTimeOnBottom = 3;

    [Header("Game Box Effects Settings")]
    public char[] possibleLetters =
    { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
        'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
}