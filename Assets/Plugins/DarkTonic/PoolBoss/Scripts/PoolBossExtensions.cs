using UnityEngine;

namespace DarkTonic.PoolBoss {
    
/// <summary>
/// Extension methods of Pool Boss methods, that you can call with one less parameter from the Transform component.
/// </summary>
public static class PoolBossExtensions
{
    /// <summary>
    /// Call this method to find out if all are despawned
    /// </summary>
    /// <param name="transPrefab">The transform of the prefab you are asking about.</param>
    /// <returns>Boolean value.</returns>
    public static bool AllOfPrefabAreDespawned(this Transform transPrefab)
    {
        return PoolBoss.AllOfPrefabAreDespawned(transPrefab);
    }

    /// <summary>
    /// This method allows you to add a new Pool Item at runtime.
    /// </summary>
    /// <param name="itemTrans">The Transform of the item.</param>
    /// <param name="preloadInstances">The number of instances to preload.</param>
    /// <param name="canInstantiateMore">Can instantiate more or not</param>
    /// <param name="hardLimit">Item Hard Limit</param>
    /// <param name="logMsgs">Log messages during spawn and despawn.</param>
    /// <param name="catName">Category name</param>
    public static void CreateNewPoolItem(this Transform itemTrans, int preloadInstances, bool canInstantiateMore,
        int hardLimit, bool logMsgs, string catName)
    {
        PoolBoss.CreateNewPoolItem(itemTrans, preloadInstances, canInstantiateMore, hardLimit, logMsgs, catName);
    }

    /// <summary>
    /// Call this method to despawn a prefab using Pool Boss. 
    /// </summary>
    /// <param name="transToDespawn">Transform to despawn</param>
    public static void Despawn(this Transform transToDespawn)
    {
        PoolBoss.Despawn(transToDespawn);
    }

    /// <summary>
    /// This method will despawn all spawned instances of the prefab you pass in.
    /// </summary>
    /// <param name="transToDespawn">Transform component of a prefab</param>
    public static void DespawnAllOfPrefab(this Transform transToDespawn)
    {
        PoolBoss.DespawnAllOfPrefab(transToDespawn);
    }

    /// <summary>
    /// This will return the name of the game object's prefab without "(Clone X)" in the name. It is used internally by PoolBoss for a lot of things.
    /// </summary>
    /// <param name="trans">The Transform of the game object</param>
    /// <returns>string</returns>
    public static string GetPrefabName(this Transform trans)
    {
        return PoolBoss.GetPrefabName(trans);
    }

    /// <summary>
    /// Call this get the next available item to spawn for a pool item.
    /// </summary>
    /// <param name="trans">Transform you want to get the next item to spawn for.</param>
    /// <returns>Transform</returns>
    public static Transform NextPoolItemToSpawn(this Transform trans)
    {
        return PoolBoss.NextPoolItemToSpawn(trans);
    }

    /// <summary>
    /// This will tell you how many available clones of a prefab are despawned and ready to spawn. A value of -1 indicates an error
    /// </summary>
    /// <param name="transPrefab">The transform component of the prefab you want the despawned count of.</param>
    /// <returns>Integer value.</returns>
    public static int PrefabDespawnedCount(this Transform transPrefab)
    {
        return PoolBoss.PrefabDespawnedCount(transPrefab);
    }

    /// <summary>
    /// Call this method determine if the item (Transform) you pass in is set up in Pool Boss.
    /// </summary>
    /// <param name="trans">Transform you want to know is in the Pool or not.</param>
    /// <returns>Boolean value.</returns>
    public static bool PrefabIsInPool(this Transform trans)
    {
        return PoolBoss.PrefabIsInPool(trans);
    }

    /// <summary>
    /// This will tell you how many clones of a prefab are already spawned out of Pool Boss. A value of -1 indicates an error
    /// </summary>
    /// <param name="transPrefab">The transform component of the prefab you want the spawned count of.</param>
    /// <returns>Integer value.</returns>
    public static int PrefabSpawnedCount(this Transform transPrefab)
    {
        return PoolBoss.PrefabSpawnedCount(transPrefab);
    }

    /// <summary>
    /// Call this method to spawn a prefab using Pool Boss, which will be a child of the Pool Boss prefab.
    /// </summary>
    /// <param name="transToSpawn">Transform to spawn</param>
    /// <param name="position">The position to spawn it at</param>
    /// <param name="rotation">The rotation to use</param>
    /// <returns>The Transform of the spawned object. It can be null if spawning failed from limits you have set.</returns>
    public static Transform SpawnInPool(this Transform transToSpawn, Vector3 position, Quaternion rotation)
    {
        return PoolBoss.SpawnInPool(transToSpawn, position, rotation);
    }

    /// <summary>
    /// Call this method to spawn a prefab using Pool Boss, which will be spawned with no parent Transform (outside the pool)
    /// </summary>
    /// <param name="transToSpawn">Transform to spawn</param>
    /// <param name="position">The position to spawn it at</param>
    /// <param name="rotation">The rotation to use</param>
    /// <returns>The Transform of the spawned object. It can be null if spawning failed from limits you have set.</returns>
    public static Transform SpawnOutsidePool(this Transform transToSpawn, Vector3 position, Quaternion rotation)
    {
        return PoolBoss.SpawnOutsidePool(transToSpawn, position, rotation);
    }

    /// <summary>
    /// Call this method to spawn a prefab using Pool Boss.
    /// </summary>
    /// <param name="transToSpawn">Transform to spawn</param>
    /// <param name="position">The position to spawn it at</param>
    /// <param name="rotation">The rotation to use</param>
    /// <param name="parentTransform">The parent Transform to use</param>
    /// <returns>The Transform of the spawned object. It can be null if spawning failed from limits you have set.</returns>
    public static Transform Spawn(this Transform transToSpawn, Vector3 position, Quaternion rotation,
        Transform parentTransform)
    {
        return PoolBoss.Spawn(transToSpawn, position, rotation, parentTransform);
    }


    /// <summary>
    /// Change the layer of something you just spawned.
    /// </summary>
    /// <param name="spawned">Spawned Transform</param>
    /// <param name="layer">Layer to put it in</param>
    public static Transform OnLayer(this Transform spawned, int layer)
    {
        spawned.GetComponent<GameObject>().layer = layer;
        return spawned;
    }

    /// <summary>
    /// Change the local scale of something you just spawned.
    /// </summary>
    /// <param name="spawned">Spawned Transform</param>
    /// <param name="newScale">The new scale to use</param>
    /// <returns></returns>
    public static Transform WithScale(this Transform spawned, Vector3 newScale)
    {
        spawned.localScale = newScale;
        return spawned;
    }
}
    
} // namespace DarkTonic.PoolBoss