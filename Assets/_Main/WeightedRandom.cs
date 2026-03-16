using System.Collections.Generic;
using UnityEngine;

public static class WeightedRandom
{
    public static T GetRandom<T>(List<WeightedValue<T>> values)
    {
        var totalWeight = 0;
        foreach (var v in values) totalWeight += v.weight;
        var random = Random.Range(0, totalWeight);
        foreach (var v in values)
        { if (random < v.weight) return v.value; random -= v.weight; }

        return values[0].value;
    }
}

[System.Serializable] public class WeightedValue<T>
{ public T value; public int weight; }