using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Utils.Extensions {

public static class LayerMaskExtensions
{
    public static bool MaskContains(this LayerMask mask, int layerNumber)
    {
        return mask == (mask | (1 << layerNumber));
    }
}

} // namespace Utils.Extensions 