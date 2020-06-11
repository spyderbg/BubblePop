using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Utils.Extensions {

public static class NumericalExtensions
{
    public static float LinearRemap(this float value,
        float valueRangeMin, float valueRangeMax,
        float newRangeMin, float newRangeMax)
    {
        return (value - valueRangeMin) / (valueRangeMax - valueRangeMin) * (newRangeMax - newRangeMin) + newRangeMin;
    }

    public static bool IsEven(this int value)
    {
        return (value & 1) == 0;
    }

    public static bool IsOdd(this int value)
    {
        return (value & 1) == 1;
    }
    
    public static int WithRandomSign(this int value, float negativeProbability = 0.5f)
    {
        return Random.value < negativeProbability ? -value : value;
    }
}

} // namespace Utils.Extensions 