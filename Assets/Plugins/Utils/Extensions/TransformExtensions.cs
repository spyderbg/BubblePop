using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace

namespace Utils.Extensions {

public static class TransformExtensions
{
    #region Position methods

    public static void SetX(this Transform self, float x)
    {
        var pos = self.transform.position;
        pos.x = x;
        self.transform.position = pos;
    }
    
    public static void SetY(this Transform self, float y)
    {
        var pos = self.transform.position;
        pos.y = y;
        self.transform.position = pos;
    }

    public static void SetZ(this Transform self, float z)
    {
        var pos = self.transform.position;
        pos.z = z;
        self.transform.position = pos;
    }
    
    public static void SetXY(this Transform self, float x, float y)
    {
        var pos = self.transform.position;
        pos.x = x;
        pos.y = y;
        self.transform.position = pos;
    }
    
    public static void SetXZ(this Transform self, float x, float z)
    {
        var pos = self.transform.position;
        pos.x = x;
        pos.z = z;
        self.transform.position = pos;
    }

    public static void SetYZ(this Transform self, float y, float z)
    {
        var pos = self.transform.position;
        pos.y = y;
        pos.z = z;
        self.transform.position = pos;
    }
    
    public static void SetXYZ(this Transform self, float x, float y, float z)
    {
        var pos = self.transform.position;
        pos.x = x;
        pos.y = y;
        pos.z = z;
        self.transform.position = pos;
    }
    
    #endregion
    
    #region LookAt methods

    /// <summary>
    /// Look at a GameObject
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look at</param>
    public static void LookAt(this Transform self, GameObject target)
    {
        self.LookAt(target.transform);
    }

    /// <summary>
    /// Find the rotation to look at a Vector3
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look at</param>
    /// <returns></returns>
    public static Quaternion GetLookAtRotation(this Transform self, Vector3 target)
    {
        return Quaternion.LookRotation(target - self.position);
    }

    /// <summary>
    /// Find the rotation to look at a Transform
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look at</param>
    /// <returns></returns>
    public static Quaternion GetLookAtRotation(this Transform self, Transform target)
    {
        return GetLookAtRotation(self, target.position);
    }

    /// <summary>
    /// Find the rotation to look at a GameObject
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look at</param>
    /// <returns></returns>
    public static Quaternion GetLookAtRotation(this Transform self, GameObject target)
    {
        return GetLookAtRotation(self, target.transform.position);
    }


    /// <summary>
    /// Instantly look away from a target Vector3
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look away from</param>
    public static void LookAwayFrom(this Transform self, Vector3 target)
    {
        self.rotation = GetLookAwayFromRotation(self, target);
    }

    /// <summary>
    /// Instantly look away from a target transform
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look away from</param>
    public static void LookAwayFrom(this Transform self, Transform target)
    {
        self.rotation = GetLookAwayFromRotation(self, target);
    }

    /// <summary>
    /// Instantly look away from a target GameObject
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look away from</param>
    public static void LookAwayFrom(this Transform self, GameObject target)
    {
        self.rotation = GetLookAwayFromRotation(self, target);
    }


    /// <summary>
    /// Find the rotation to look away from a target Vector3
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look away from</param>
    public static Quaternion GetLookAwayFromRotation(this Transform self, Vector3 target)
    {
        return Quaternion.LookRotation(self.position - target);
    }

    /// <summary>
    /// Find the rotation to look away from a target transform
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look away from</param>
    public static Quaternion GetLookAwayFromRotation(this Transform self, Transform target)
    {
        return GetLookAwayFromRotation(self, target.position);
    }

    /// <summary>
    /// Find the rotation to look away from a target GameObject
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">The thing to look away from</param>
    public static Quaternion GetLookAwayFromRotation(this Transform self, GameObject target)
    {
        return GetLookAwayFromRotation(self, target.transform.position);
    }

    #endregion

    #region Child methods

    public static void DestroyChildren(this Transform self, bool immediate = false)
    {
        if (immediate)
        {
            for(var i = 0; i < self.childCount; i++)
                Object.DestroyImmediate(self.GetChild(i).gameObject);
        }
        else
        {
            for(var i = 0; i < self.childCount; i++)
                Object.Destroy(self.GetChild(i).gameObject);
            
        }
    }

    #endregion
}

} // namespace Utils.Extensions 