using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<float> modifiers = new();

    public float GetValue() 
    {
        float finalValue = baseValue;
        foreach (var modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }   

    public void SetValue(float value) 
    {
        baseValue = value;
    } 

    public float ReturnBaseValue() 
    {
        return baseValue;
    }
}
