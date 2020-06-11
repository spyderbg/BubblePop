using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Utils.Extensions {

public static class RectExtensions
{
    public static bool ContainsInclusive(this Rect self, Vector2 point)
    {
      return (double) point.x >= (double) self.xMin 
             && (double) point.x <= (double) self.xMax
             && (double) point.y >= (double) self.yMin
             && (double) point.y <= (double) self.yMax;
    }

}

} // namespace Utils.Extensions 
