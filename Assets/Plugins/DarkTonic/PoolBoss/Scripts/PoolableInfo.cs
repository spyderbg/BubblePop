using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.PoolBoss {
public class PoolableInfo : MonoBehaviour
{
    public string PoolItemName = string.Empty;

    void OnSpawned()
    {
        PoolBoss.UnregisterNonStartInScenePoolable(this);
    }

    void OnEnable()
    {
        PoolBoss.RegisterPotentialInScenePoolable(this);
    }

    void OnDisable()
    {
        PoolBoss.UnregisterNonStartInScenePoolable(this);
    }

    void Reset()
    {
        if (!Application.isPlaying)
        {
            FindPoolItemName();
        }
    }

    public void FindPoolItemName()
    {
        if (!string.IsNullOrEmpty(PoolItemName))
        {
            return;
        }

        PoolItemName = PoolBoss.GetPrefabShortName(name);
    }

    /// <summary>
    /// This will get called instead by other scripts if you already know the name
    /// </summary>
    /// <param name="itemName"></param>
    public void SetPoolItemName(string itemName)
    {
        PoolItemName = itemName;
    }

    public string ItemName {
        get {
            if (string.IsNullOrEmpty(PoolItemName))
                FindPoolItemName();
            return PoolItemName;
        }
    }
}
    
} // namespace DDarkTonic.PoolBoss