using System;

// ReSharper disable once CheckNamespace
namespace DarkTonic.PoolBoss {
    
[Serializable]
// ReSharper disable once CheckNamespace
public class PoolBossCategory
{
    public string CatName = PoolBoss.NoCategory;
    public bool IsExpanded = true;
    public bool IsEditing = false;
    public string ProspectiveName = PoolBoss.NoCategory;
}
    
} // namespace DarkTonic.PoolBoss 
