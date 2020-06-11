using System;
using UnityEngine;

#if ADDRESSABLES_ENABLED
    using UnityEngine.AddressableAssets;
#endif

// ReSharper disable once CheckNamespace
namespace DarkTonic.PoolBoss {
    
[Serializable]
// ReSharper disable once CheckNamespace
public class PoolBossItem
{
    // ReSharper disable InconsistentNaming
    public PoolBoss.PrefabSource prefabSource;
    public Transform prefabTransform;
#if ADDRESSABLES_ENABLED
    public AssetReference prefabAddressable;
#endif
    public int instancesToPreload = 1;
    public bool isExpanded = true;
    public bool logMessages;
    public bool enableNavMeshAgentOnSpawn = false;
    public bool allowInstantiateMore;
    public int delayNavMeshEnableByFrames = 1;
    public int itemHardLimit = 10;
    public bool allowRecycle;

    public string categoryName = PoolBoss.NoCategory;
    // ReSharper restore InconsistentNaming

    public PoolBossItem Clone()
    {
        var clone = new PoolBossItem
        {
            prefabSource = prefabSource,
            prefabTransform = prefabTransform,
            instancesToPreload = instancesToPreload,
            isExpanded = isExpanded,
            logMessages = logMessages,
            allowInstantiateMore = allowInstantiateMore,
            itemHardLimit = itemHardLimit,
            allowRecycle = allowRecycle,
            categoryName = categoryName,
            delayNavMeshEnableByFrames = delayNavMeshEnableByFrames
        };

#if ADDRESSABLES_ENABLED
        if (prefabAddressable.RuntimeKeyIsValid())
        {
            clone.prefabAddressable = new AssetReference(prefabAddressable.RuntimeKey.ToString());
        }
#endif

        return clone;
    }
}
    
} // namespace DarkTonic.PoolBoss