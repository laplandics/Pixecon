using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BoxEffect
{
    public string name;
    public int priority;
    public GameBox owner;

    public virtual void Init(GameBox box) { owner = box; }

    public virtual IEnumerator GetReady() { return null; }
    public abstract IEnumerator Perform();

    protected Texture2D LoadEffectSprite(string path, string effectName)
    {
        var textures = Resources.LoadAll<Texture2D>(path);
        foreach (var texture2D in textures) { if (texture2D.name == effectName) return texture2D; }
        return textures[^1];
    }

    protected void UpdateBoxLayer(Texture2D texture)
    {
        var layerName = $"_Layer{owner.State.TextureSpotsUsed}";
        var index = owner.State.TextureSpotsUsed + 1;
        var layersOnObj = new string[4];
        for (var i = 0; i < index; i++) { layersOnObj[i] = $"_Layer{i}"; }
        
        owner.State.BoxMatBlock.SetTexture(layerName, texture);
        owner.State.BoxMatBlock.SetFloat($"_Toggle{layerName}", 1);
        
        var preset = Settings.Get.effectsLayoutPresets[index - 1];
        var tilings = preset.tilings;
        var offsets = preset.offsets;
        
        for (var i = 0; i < index; i++)
        {
            var tiling = tilings[i];
            var offset = offsets[i];
            
            owner.State.BoxMatBlock.SetFloat($"{layersOnObj[i]}TilingX", tiling.x);
            owner.State.BoxMatBlock.SetFloat($"{layersOnObj[i]}TilingY", tiling.y);
            owner.State.BoxMatBlock.SetFloat($"{layersOnObj[i]}OffsetX", offset.x);
            owner.State.BoxMatBlock.SetFloat($"{layersOnObj[i]}OffsetY", offset.y);
        }
        owner.State.BoxMeshRenderer.SetPropertyBlock(owner.State.BoxMatBlock);
        owner.State.TextureSpotsUsed++;
    }
}

[Serializable]
public class SetLetter : BoxEffect
{
    private const string PATH = "Textures/Effects/Letters/";
    private const int BASE_PRIORITY = 90;
    private char _printedLetter;
    
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
        LoadLetter();
    }

    private void LoadLetter()
    {
        var currentWordLetters = Core.Observer.Get<Game>().WordLetters;
        if (currentWordLetters is not { Count: > 0 }) return;
        var allLetters = Settings.Get.possibleLetters;
        var lettersWeighted = new List<WeightedValue<char>>();
        foreach (var letter in allLetters)
        {
            var weightedValue = new WeightedValue<char>();
            weightedValue.value = letter;
            foreach (var wordLetter in currentWordLetters)
            {
                if (wordLetter.Letter == letter)
                { weightedValue.weight = 85; priority = 100; break; }
                weightedValue.weight = 15;
            }
            lettersWeighted.Add(weightedValue);
        }

        var letterName = WeightedRandom.GetRandom(lettersWeighted);
        _printedLetter = letterName;
        var texture2D = LoadEffectSprite(PATH, $"{letterName}");
        UpdateBoxLayer(texture2D);
    }

    public override IEnumerator Perform()
    {
        var currentWordLetters = Core.Observer.Get<Game>().WordLetters;
        foreach (var wordLetter in currentWordLetters)
        {
            if (wordLetter.IsEntered) continue;
            if (wordLetter.Letter == _printedLetter)
            {
                wordLetter.IsEntered = true;
                Core.Event.Invoke(new CorrectLetterEnteredEvent { Letter = wordLetter.Letter });
            }
            break;
        }

        yield break;
    }
}

[Serializable]
public class SetScore : BoxEffect
{
    private const int BASE_PRIORITY = 200;
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}

[Serializable]
public class SetRandomLetter : BoxEffect
{
    private const int BASE_PRIORITY = 80;
    
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}

[Serializable]
public class SetCorrectLetter : BoxEffect
{
    private const int BASE_PRIORITY = 100;
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}

[Serializable]
public class Explode : BoxEffect
{
    private const int BASE_PRIORITY = 20;
    
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}

[Serializable]
public class RandomizeField : BoxEffect
{
    private const int BASE_PRIORITY = 10;
    
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}

[Serializable]
public class PreformRandomEffect : BoxEffect
{
    private const int BASE_PRIORITY = 10;
    
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}

[Serializable]
public class SetChosenLetter : BoxEffect
{
    private const int BASE_PRIORITY = 300;
    
    public override void Init(GameBox box)
    {
        base.Init(box);
        priority = BASE_PRIORITY;
    }
    
    public override IEnumerator Perform()
    {
        
        yield break;
    }
}