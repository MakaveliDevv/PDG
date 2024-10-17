using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<float> modifiers = new();

    [Header("For The Enemy")]
    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;

    public float GetValue() 
    {
        float finalValue = baseValue;
        foreach (var modifier in modifiers)
        {
            finalValue += modifier;
        }

        return finalValue;
    }   

    public void SetValue(int value) 
    {
        baseValue = value;
    }

    public void SetMinMaxValue(int minValue, int maxValue) 
    {
        this.minValue = minValue;
        this.maxValue = maxValue;
    } 

    
    public int ReturnMinValue() 
    {
        return minValue;
    }

    public int ReturnMaxValue() 
    {
        return maxValue;
    }

    public float ReturnBaseValue() 
    {
        return baseValue;
    }
}
