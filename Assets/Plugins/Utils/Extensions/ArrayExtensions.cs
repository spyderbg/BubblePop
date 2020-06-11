using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Utils.Extensions {

public static class ArrayExtensions
{
    /// <summary>
    /// For each component in an array, take an action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="callback">The action to take</param>
    public static void ForEachComponent<T>(this T[] array, [NotNull] System.Action<T> callback) where T : Component
    {
        if(callback == null) throw new ArgumentNullException( nameof(callback) );

        foreach(var t in array)
            callback.Invoke(t);
    }

    /// <summary>
    /// Implement cycle ElementAt for an array. Don't not check for overflow of integer type.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="index">Index of the element of the array. Negative number are accept also.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>Returns cycle element</returns>
    /// <exception cref="ArgumentException"></exception>
    public static T CycleElementAt<T>(this T[] array, int index)
    {
        if(array == null || array.Length == 0) throw new ArgumentException( nameof(array) );

        var len = array.Length;

        while(index < 0) index += len;
        while(index > len - 1) index -= len;

        return array.ElementAt( index );
    }
}

} // namespace Utils.Extensions 