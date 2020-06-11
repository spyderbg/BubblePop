// ReSharper disable once CheckNamespace

using TMPro.SpriteAssetUtilities;

namespace DarkTonic.PoolBoss.Editor {
    
public class PoolBossLang
{
    public static readonly string DoNotDestroyPoolItem = 
        "This will destroy the Pool Item. Pool Items should only be despawned, never destroyed.";

    // Error messages
    public static readonly string ErrorInProjectView =
        "You have selected the PoolBoss prefab in Project View. Please select the one in your Scene to edit. " +
        "Or, to create one in your Scene, drag this into the Hierarchy. " +
        "But make your own prefab out of it so you won't get your changed overridden next time you update Pool Boss.";

    public static readonly string ErrorBlankCategoryName =
        "You cannot have a blank Category name.";

    public static readonly string ErrorDuplicateCategoryName =
        "You already have a Category named '{0}'. Category names must be unique.";
    
    // Alert messages 
    public static readonly string EmptyCategory =
        "This Category is empty. Add / move some items or you may delete it.";

    public static readonly string PrefabContainsTrail =
        "This prefab contains a Trail Renderer with auto-destruct enabled. {0}";

    public static readonly string PreloadQtyToZero =
        "You have set the Preload Qty to 0. This prefab will not be in the Pool.";

    public static readonly string DeleteCategoryWithPool =
        "You cannot delete a Category with Pool Items in it. Move or delete the items first.";

    public static readonly string DeleteLastCategory =
        "You cannot delete the last Category.";

    public static readonly string BlankCategoryName =
        "You cannot have a blank Category name.";

    public static readonly string CategoryNameUnique =
        "You already have a Category named '{0}'. Category names must be unique.";

    public static readonly string DraggedNotGameObject =
        "You dragged an object which was not a Game Object. Not adding to Pool Boss.";
    
    // UI messages
    public static readonly string NewCategoryName = "New Category Name";
    
    public static readonly string ChangeNewCategoryName = "change New Category Name";
    
    public static readonly string CreateNewCategory = "Create New Category";
    
    public static readonly string DefaultItemCategory = "Default Item Category";
    
    public static readonly string ChangeDefaultItemCategory = "change Default Item Category";

    public static readonly string InitializeTime = "Initialize Time (Frames)";
    
    public static readonly string InitializeTimeChange = "change Initialize Time (Frames)";
     
    public static readonly string InitializeTimeDescription =
        "You can increase this value to make the initial pool creation take more frames. Defaults to 1. " +
        "Max of the 100 or number of different prefabs, whichever is less.";

    public static readonly string  AutoAddMissing = "Auto-Add Missing Items";
    
    public static readonly string  AutoAddMissingCategory = "Auto-Add Missing Items to top Category";

    public static readonly string  AutoAddToggle = "toggle Auto-Add Missing Items";

    public static readonly string DisabledDespawn = "Can Disabled Obj. Despawn";
    
    public static readonly string DisabledDespawnDescription = "Allow Disabled Game Objects To Despawn";
    
    public static readonly string DisabledDespawnToggle = "toggle Can Disabled Obj. Despawn";

    public static readonly string  LogMessages = "Log Messages";
    
    public static readonly string  LogMessagesToggle = "toggle Log Messages";

    public static readonly string ClearPeaks = "Clear Peaks";

    public static readonly string CollapseAll = "Collapse All";
    
    public static readonly string CollapseAllTooltip = "Click to collapse all categories and items";
    
    public static readonly string ExpandAll = "Expand All";
    
    public static readonly string ExpandAllTooltip = "Click to expand all categories and items";
    
    public static readonly string DespawnAll = "Despawn All";
    
    public static readonly string DespawnAllTooltip = "Click to despawn prefabs";

    public static readonly string DespawnAllPrefabsTooltip = "Click to despawn all prefabs in this Category";
    
    public static readonly string DespawnAllPrefabs = "Click to despawn all of this prefab";
    
    public static readonly string DragPrefabHere = "Drag prefabs here in bulk to add them to the Pool!";

    public static readonly string ExpandCategory = "toggle expand Pool Boss Category";

    public static readonly string ExpandAllItems = "Click to expand all items in this category";
    
    public static readonly string CollapseAllItems = "Click to collapse all items in this category";

    public static readonly string Expand = "Expand";
    
    public static readonly string Collapse = "Collapse";

    public static readonly string ShiftCategoryUp = "Click to shift Category up";

    public static readonly string ShiftCategoryDown = "Click to shift Category down";

    public static readonly string EditCategory = "Click to edit Category";

    public static readonly string DeleteCategory = "Delete Category";
    
    public static readonly string ClickDeleteCategory = "Click to delete Category";

    public static readonly string Delete = "Delete";

    public static readonly string Category = "category";

    public static readonly string ToggleExpandPool = "toggle expand Pool Item";

    public static readonly string SpawnedCount = "{0} / {1} Spawned";
    
    public static readonly string SpawnedCountTooltip = "Click here to select all spawned items.";

    public static readonly string ResetPeak = "Click to reset peak to zero.";

    public static readonly string ChangePoolItemCategory = "change Pool Item Category";

    public static readonly string PoolItemPrefab = "Pool Item prefab";
    
    public static readonly string PoolItem = "Pool Item";

    public static readonly string Prefab = "Prefab";

    public static readonly string ChangePoolItemPrefab = "change Pool Item Prefab";

    public static readonly string PrefabAddressable = "Prefab Addressable";
    
    public static readonly string PrefabAddressableTooltip = "Drag an Addressable prefab here";

    public static readonly string PreloadQty = "Preload Qty";
    
    public static readonly string ChangePreloadQty = "change Pool Item Preload Qty";

    public static readonly string AllowInstantiate = "Allow Instantiate More";

    public static readonly string ToggleAllowInstantiate = "toggle Allow Instantiate More";

    public static readonly string ItemLimit = "Item Limit";
    
    public static readonly string ChangeItemLimit = "change Item Limit";

    public static readonly string RecycleOldest = "Recycle Oldest";
    
    public static readonly string ToggleRecycleOldest = "toggle Recycle Oldest";

    public static readonly string EnableNavMesh = "Enable NavMeshAgent";
    
    public static readonly string ToggleEnableNavMesh = "toggle Enable NavMeshAgent On Spawn";
    
    public static readonly string EnableNavMeshTooltip = "Check this to enable NavMeshAgent component whenever spawned";

    public static readonly string NavMeshAgentDelay = "NavMeshAgent Frames Delay";
    
    public static readonly string ChangeNavMeshAgentDelay = "change NavMeshAgent Frames Delay";

    public static readonly string ShiftUp = "shift up Category";

    public static readonly string ShiftDown = "shift down Category";

    public static readonly string UndoChangeCategory = "Undo change Category name.";

    public static readonly string RemovePoolItem = "remove Pool Item";

    public static readonly string ClonePoolItem = "clone Pool Item";

    public static readonly string InsertPoolItem = "insert Pool Item";

    public static readonly string ToggleItems = "toggle expand / collapse all Pool Boss Items";
    
    public static readonly string ToggleItemsInCategory = "toggle expand / collapse all items in Category";

    public static readonly string AddPoolIem = "add Pool Boss Item";
}

} // namespace DarkTonic.PoolBoss.Editor