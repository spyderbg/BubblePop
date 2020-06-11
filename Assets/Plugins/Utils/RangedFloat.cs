using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RangedFloat
{
    public float minValue;
    public float maxValue;

    public RangedFloat()
    {
        minValue = maxValue = 0.0f;
    }

    public RangedFloat(float val)
    {
        minValue = maxValue = val;
    }

    public RangedFloat(float min, float max)
    {
        minValue = min;
        maxValue = max;
    }
    
    public float Range => maxValue - minValue;
    public float RandomValue => minValue + Random.value * Range;

    public bool IsInRange(float val) => minValue <= val && val <= maxValue;

    public float Clamp(float val) => Mathf.Clamp(val, minValue, maxValue);

    public void Translate(float val)
    {
        minValue += val;
        maxValue += val;
    }
    
    public static RangedFloat operator +(RangedFloat f, float val)
    {
        f.minValue -= val;
        f.maxValue += val;
        return f;
    }
    
    public static RangedFloat operator -(RangedFloat f, float val)
    {
        f.minValue += val;
        f.maxValue -= val;
        return f;
    }
}
