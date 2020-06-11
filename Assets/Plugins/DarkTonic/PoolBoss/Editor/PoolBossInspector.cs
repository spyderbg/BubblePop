using System;
using System.Collections.Generic;
using DarkTonic.PoolBoss;
using DarkTonic.PoolBoss.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_5_5 || UNITY_5_6 || UNITY_2017_1_OR_NEWER
using UnityEngine.AI;

#endif

[CustomEditor(typeof(PoolBoss))]
// ReSharper disable once CheckNamespace
public class PoolBossInspector : Editor
{
    private PoolBoss _pool;
    private bool _isDirty;

    // ReSharper disable once FunctionComplexityOverflow
    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;

        _pool = (PoolBoss) target;

        _isDirty = false;

        var isInProjectView = DTPoolBossInspectorUtility.IsPrefabInProjectView(_pool);
        if (isInProjectView)
        {
            DTPoolBossInspectorUtility.ShowRedError(PoolBossLang.ErrorInProjectView);
            return;
        }

        var catNames = new List<string>(_pool._categories.Count);
        for (var i = 0; i < _pool._categories.Count; i++)
            catNames.Add(_pool._categories[i].CatName);

        if (!Application.isPlaying)
        {
            DTPoolBossInspectorUtility.StartGroupHeader();
            var newCat = EditorGUILayout.TextField(PoolBossLang.NewCategoryName, _pool.newCategoryName);
            if (newCat != _pool.newCategoryName)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ChangeNewCategoryName);
                _pool.newCategoryName = newCat;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginHorizontal();
            GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;
            GUILayout.Space(2);
            if (GUILayout.Button(PoolBossLang.CreateNewCategory, EditorStyles.toolbarButton, GUILayout.Width(130)))
                CreateCategory();

            GUI.contentColor = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            DTPoolBossInspectorUtility.AddSpaceForNonU5();
            DTPoolBossInspectorUtility.ResetColors();

            var selCatIndex = catNames.IndexOf(_pool.addToCategoryName);

            if (selCatIndex == -1)
            {
                selCatIndex = 0;
                _isDirty = true;
            }

            GUI.backgroundColor = DTPoolBossInspectorUtility.BrightButtonColor;

            var newIndex = EditorGUILayout.Popup(PoolBossLang.DefaultItemCategory, selCatIndex, catNames.ToArray());
            if (newIndex != selCatIndex)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ChangeDefaultItemCategory);
                _pool.addToCategoryName = catNames[newIndex];
            }

            GUI.backgroundColor = Color.white;
            GUI.contentColor = Color.white;
        }

        GUI.contentColor = Color.white;

        if (!Application.isPlaying)
        {
            var maxFrames = Math.Max(1, _pool.poolItems.Count);
            var guiContent = new GUIContent(PoolBossLang.InitializeTime, PoolBossLang.InitializeTimeDescription);
            var newFrames = EditorGUILayout.IntSlider(guiContent, _pool.framesForInit, 1, maxFrames);
            if (newFrames != _pool.framesForInit)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.InitializeTimeChange);
                _pool.framesForInit = newFrames;
            }
        }

        PoolBossItem itemToRemove = null;
        int? indexToInsertAt = null;
        PoolBossCategory selectedCategory = null;
        PoolBossItem itemToClone = null;

        PoolBossCategory catEditing = null;
        PoolBossCategory catRenaming = null;

        PoolBossCategory catToDelete = null;
        int? indexToShiftUp = null;
        int? indexToShiftDown = null;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool.poolItems.Count; i++)
        {
            var item = _pool.poolItems[i];
            if (catNames.Contains(item.categoryName))
                continue;

            item.categoryName = catNames[0];
            _isDirty = true;
        }

        var newAutoAdd = EditorGUILayout.Toggle(new GUIContent(PoolBossLang.AutoAddMissing, PoolBossLang.AutoAddMissingCategory), _pool.autoAddMissingPoolItems);
        if (newAutoAdd != _pool.autoAddMissingPoolItems)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.AutoAddToggle);
            _pool.autoAddMissingPoolItems = newAutoAdd;
        }

        var newAllowDisabled = EditorGUILayout.Toggle(new GUIContent(PoolBossLang.DisabledDespawn, PoolBossLang.DisabledDespawnDescription), _pool.allowDespawningInactive);
        if (newAllowDisabled != _pool.allowDespawningInactive)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.DisabledDespawnToggle);
            _pool.allowDespawningInactive = newAllowDisabled;
        }

        var newLog = EditorGUILayout.Toggle(PoolBossLang.LogMessages, _pool.logMessages);
        if (newLog != _pool.logMessages)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.LogMessagesToggle);
            _pool.logMessages = newLog;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Actions", GUILayout.Width(100));
        GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;

        GUILayout.FlexibleSpace();

        var allExpanded = true;
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool._categories.Count; i++)
        {
            if (_pool._categories[i].IsExpanded)
            {
                continue;
            }

            allExpanded = false;
            break;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var i = 0; i < _pool.poolItems.Count; i++)
        {
            if (_pool.poolItems[i].isExpanded)
            {
                continue;
            }

            allExpanded = false;
            break;
        }

        if (Application.isPlaying)
        {
            if (GUILayout.Button(new GUIContent(PoolBossLang.ClearPeaks), EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                ClearAllPeaks();
                _isDirty = true;
            }

            GUILayout.Space(6);
        }

        var buttonTooltip = allExpanded ? PoolBossLang.CollapseAllTooltip : PoolBossLang.ExpandAllTooltip;
        var buttonText = allExpanded ? PoolBossLang.CollapseAll : PoolBossLang.ExpandAll;
        if (GUILayout.Button(new GUIContent(buttonText, buttonTooltip), EditorStyles.toolbarButton,
            GUILayout.Width(80)))
        {
            ExpandCollapseAll(!allExpanded);
        }

        if (Application.isPlaying)
        {
            GUILayout.Space(6);
            if (GUILayout.Button(new GUIContent(PoolBossLang.DespawnAll, PoolBossLang.DespawnAllTooltip), EditorStyles.toolbarButton,
                GUILayout.Width(80)))
            {
                PoolBoss.DespawnAllPrefabs();
                _isDirty = true;
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        GUI.backgroundColor = Color.white;

        if (!Application.isPlaying)
        {
#if ADDRESSABLES_ENABLED
            var newSource =
 (PoolBoss.PrefabSource)EditorGUILayout.EnumPopup("Create Items As", _pool.newItemPrefabSource);
            if (newSource != _pool.newItemPrefabSource)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Create Items As");
                _pool.newItemPrefabSource = newSource;
            }
#endif

            EditorGUILayout.BeginVertical();
            var anEvent = Event.current;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(4);
            GUI.color = DTPoolBossInspectorUtility.DragAreaColor;
            var dragArea = GUILayoutUtility.GetRect(0f, 30f, GUILayout.ExpandWidth(true));
            GUI.Box(dragArea, PoolBossLang.DragPrefabHere);
            GUI.color = Color.white;

            switch (anEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!dragArea.Contains(anEvent.mousePosition))
                    {
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (anEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var dragged in DragAndDrop.objectReferences)
                        {
                            AddPoolItem(dragged);
                        }
                    }

                    Event.current.Use();
                    break;
            }

            GUILayout.Space(4);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            DTPoolBossInspectorUtility.VerticalSpace(4);
        }

        GUI.backgroundColor = Color.white;
        GUI.contentColor = Color.white;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var c = 0; c < _pool._categories.Count; c++)
        {
            var cat = _pool._categories[c];

            EditorGUI.indentLevel = 0;

            var matchingItems = new List<PoolBossItem>();
            matchingItems.AddRange(_pool.poolItems);
            matchingItems.RemoveAll(delegate(PoolBossItem x) { return x.categoryName != cat.CatName; });

            var hasItems = matchingItems.Count > 0;

            EditorGUILayout.BeginHorizontal();

            if (!cat.IsEditing || Application.isPlaying)
            {
                var catName = cat.CatName;

                if (!cat.IsExpanded)
                {
                    catName += $": {matchingItems.Count} item{((matchingItems.Count != 1) ? "s" : "")}";
                }

                var state = cat.IsExpanded;
                var text = catName;

                DTPoolBossInspectorUtility.ShowCollapsibleSectionInline(ref state, text);
                GUILayout.Space(2f);

                if (state != cat.IsExpanded)
                {
                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ExpandCategory);
                    cat.IsExpanded = state;
                }

                EditorGUILayout.EndHorizontal();

                var catItemsCollapsed = true;

                for (var i = 0; i < _pool.poolItems.Count; i++)
                {
                    var item = _pool.poolItems[i];
                    if (item.categoryName != cat.CatName)
                    {
                        continue;
                    }

                    if (!item.isExpanded)
                    {
                        continue;
                    }

                    catItemsCollapsed = false;
                    break;
                }

                var headerStyle = new GUIStyle();
                headerStyle.margin = new RectOffset(0, 0, 0, 0);
                headerStyle.padding = new RectOffset(0, 0, 1, 0);
                headerStyle.fixedHeight = 20;

                EditorGUILayout.BeginHorizontal(headerStyle, GUILayout.MaxWidth(50));

                GUI.backgroundColor = Color.white;

                var tooltip = catItemsCollapsed ? PoolBossLang.ExpandAllItems : PoolBossLang.CollapseAllItems;
                var btnText = catItemsCollapsed ? PoolBossLang.Expand : PoolBossLang.Collapse;

                GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;
                if (GUILayout.Button(new GUIContent(btnText, tooltip), EditorStyles.toolbarButton, GUILayout.Width(60),
                    GUILayout.Height(16)))
                {
                    ExpandCollapseCategory(cat.CatName, catItemsCollapsed);
                }

                GUI.contentColor = Color.white;

                if (!Application.isPlaying)
                {
                    if (c > 0)
                    {
                        // the up arrow.
                        var upArrow = PoolBossInspectorResources.UpArrowTexture;
                        if (GUILayout.Button(new GUIContent(upArrow, PoolBossLang.ShiftCategoryUp),
                            EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16)))
                        {
                            indexToShiftUp = c;
                        }
                    }
                    else
                    {
                        GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                    }

                    if (c < _pool._categories.Count - 1)
                    {
                        // The down arrow will move things towards the end of the List
                        var dnArrow = PoolBossInspectorResources.DownArrowTexture;
                        if (GUILayout.Button(new GUIContent(dnArrow, PoolBossLang.ShiftCategoryDown),
                            EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16)))
                        {
                            indexToShiftDown = c;
                        }
                    }
                    else
                    {
                        GUILayout.Button("", EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16));
                    }

                    var settingsIcon = new GUIContent(PoolBossInspectorResources.SettingsTexture, PoolBossLang.EditCategory);

                    GUI.backgroundColor = Color.white;
                    if (GUILayout.Button(settingsIcon, EditorStyles.toolbarButton, GUILayout.Width(24), GUILayout.Height(16)))
                    {
                        catEditing = cat;
                    }

                    GUI.backgroundColor = DTPoolBossInspectorUtility.DeleteButtonColor;
                    if (GUILayout.Button(new GUIContent(PoolBossLang.Delete, PoolBossLang.ClickDeleteCategory),
                        EditorStyles.miniButton, GUILayout.MaxWidth(51)))
                    {
                        catToDelete = cat;
                    }

                    GUILayout.Space(15);
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    GUI.contentColor = DTPoolBossInspectorUtility.BrightButtonColor;

                    if (GUILayout.Button(new GUIContent(PoolBossLang.DespawnAll, PoolBossLang.DespawnAllPrefabsTooltip),
                        EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        PoolBoss.DespawnAllPrefabsInCategory(cat.CatName);
                        _isDirty = true;
                    }

                    var itemsSpawned = PoolBoss.CategoryItemsSpawned(cat.CatName);
                    var categoryHasItemsSpawned = itemsSpawned > 0;
                    var theBtnText = itemsSpawned.ToString();
                    var btnColor = categoryHasItemsSpawned
                        ? DTPoolBossInspectorUtility.BrightTextColor
                        : DTPoolBossInspectorUtility.DeleteButtonColor;
                    GUI.backgroundColor = btnColor;

                    var btnWidth = 32;
                    if (theBtnText.Length > 3)
                    {
                        btnWidth = 11 * theBtnText.Length;
                    }

                    GUILayout.Button(theBtnText, EditorStyles.miniButtonRight, GUILayout.MaxWidth(btnWidth));

                    GUI.backgroundColor = Color.white;
                    GUI.contentColor = Color.white;
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                GUI.backgroundColor = DTPoolBossInspectorUtility.BrightTextColor;
                var tex = EditorGUILayout.TextField("", cat.ProspectiveName);
                if (tex != cat.ProspectiveName)
                {
                    cat.ProspectiveName = tex;
                    _isDirty = true;
                }

                var buttonPressed = DTPoolBossInspectorUtility.AddCancelSaveButtons(PoolBossLang.Category);

                switch (buttonPressed)
                {
                    case DTPoolBossInspectorUtility.FunctionButtons.Cancel:
                        cat.IsEditing = false;
                        cat.ProspectiveName = cat.CatName;
                        _isDirty = true;
                        break;
                    case DTPoolBossInspectorUtility.FunctionButtons.Save:
                        catRenaming = cat;
                        _isDirty = true;
                        break;
                }

                GUILayout.Space(4);
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            if (cat.IsEditing)
            {
                DTPoolBossInspectorUtility.VerticalSpace(2);
            }

            matchingItems.Sort(delegate(PoolBossItem x, PoolBossItem y)
            {
                return string.Compare(PoolBossItemName(x), PoolBossItemName(y), StringComparison.Ordinal);
            });

            if (!hasItems)
            {
                DTPoolBossInspectorUtility.BeginGroupedControls();
                DTPoolBossInspectorUtility.ShowLargeBarAlert(PoolBossLang.EmptyCategory);
                DTPoolBossInspectorUtility.EndGroupedControls();
            }

            if (cat.IsExpanded)
            {
                if (matchingItems.Count > 0)
                {
                    DTPoolBossInspectorUtility.BeginGroupedControls();
                }

                for (var i = 0; i < matchingItems.Count; i++)
                {
                    var poolItem = matchingItems[i];

                    DTPoolBossInspectorUtility.StartGroupHeader();

                    if (poolItem.prefabTransform != null)
                    {
                        var rend = poolItem.prefabTransform.GetComponent<TrailRenderer>();
                        if (rend != null && rend.autodestruct)
                        {
                            DTPoolBossInspectorUtility.ShowRedError(string.Format(PoolBossLang.PrefabContainsTrail, PoolBossLang.DoNotDestroyPoolItem));
                        }
                        else
                        {
#if UNITY_5_4_OR_NEWER
                            // nothing to check
#else
                            var partAnim = poolItem.prefabTransform.GetComponent<ParticleAnimator>();
                            if (partAnim != null && partAnim.autodestruct) {
								DTPoolBossInspectorUtility.ShowRedError(
                                    $"This prefab contains a Particle Animator with auto-destruct enabled. {PoolBossLang.DoNotDestroyPoolItem}");
                            }
#endif
                        }
                    }

                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.BeginHorizontal();
                    string itemName = string.Empty;

                    switch (poolItem.prefabSource)
                    {
                        case PoolBoss.PrefabSource.Prefab:
                            itemName = poolItem.prefabTransform == null ? "[NO PREFAB]" : poolItem.prefabTransform.name;
                            break;
#if ADDRESSABLES_ENABLED
                        case PoolBoss.PrefabSource.Addressable:
                            var addressableName =
 PoolBossAddressableEditorHelper.EditTimeAddressableName(poolItem.prefabAddressable);
                            itemName = string.IsNullOrWhiteSpace(addressableName) ? "[NO PREFAB]" : addressableName;
                            break;
#endif
                    }

                    var state = DTPoolBossInspectorUtility.Foldout(poolItem.isExpanded, itemName);
                    if (state != poolItem.isExpanded)
                    {
                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ToggleExpandPool);
                        poolItem.isExpanded = state;
                    }

                    if (Application.isPlaying)
                    {
                        GUILayout.FlexibleSpace();

                        GUI.contentColor = DTPoolBossInspectorUtility.BrightTextColor;
                        if (poolItem.prefabTransform != null)
                        {
                            if (GUILayout.Button(new GUIContent(PoolBossLang.DespawnAll, PoolBossLang.DespawnAllPrefabs),
                                EditorStyles.toolbarButton, GUILayout.Width(80)))
                            {
                                PoolBoss.DespawnAllOfPrefab(poolItem.prefabTransform);
                                _isDirty = true;
                            }

                            var itemInfo = PoolBoss.PoolItemInfoByName(itemName);
                            if (itemInfo != null)
                            {
                                var spawnedCount = itemInfo.SpawnedClones.Count;
                                var despawnedCount = itemInfo.DespawnedClones.Count;
                                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                                if (spawnedCount == 0)
                                {
                                    GUI.backgroundColor = DTPoolBossInspectorUtility.DeleteButtonColor;
                                }
                                else
                                {
                                    GUI.backgroundColor = DTPoolBossInspectorUtility.BrightTextColor;
                                }
                                
                                var spawned = string.Format(PoolBossLang.SpawnedCount, spawnedCount, despawnedCount + spawnedCount);
                                var content = new GUIContent(spawned, PoolBossLang.SpawnedCountTooltip);
                                if (GUILayout.Button(content, EditorStyles.toolbarButton, GUILayout.Width(110)))
                                {
                                    var obj = new List<GameObject>();
                                    foreach (var t in itemInfo.SpawnedClones)
                                    {
                                        obj.Add(t.gameObject);
                                    }

                                    if (obj.Count > 0)
                                    {
                                        Selection.objects = obj.ToArray();
                                    }
                                }

                                content = new GUIContent("Pk: " + itemInfo.Peak, PoolBossLang.ResetPeak);
                                if (Time.realtimeSinceStartup - itemInfo.PeakTime < .2f)
                                {
                                    GUI.backgroundColor = DTPoolBossInspectorUtility.AddButtonColor;
                                }
                                else if (itemInfo.Peak == 0)
                                {
                                    GUI.backgroundColor = DTPoolBossInspectorUtility.DeleteButtonColor;
                                }
                                else
                                {
                                    GUI.backgroundColor = DTPoolBossInspectorUtility.BrightTextColor;
                                }

                                if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Width(64)))
                                {
                                    itemInfo.Peak = Math.Max(0, itemInfo.SpawnedClones.Count);
                                    itemInfo.PeakTime = Time.realtimeSinceStartup;
                                    _isDirty = true;
                                    _pool._changes++;
                                }

                                GUI.backgroundColor = Color.white;
                            }
                        }

                        GUI.contentColor = Color.white;
                    }
                    else
                    {
                        GUI.backgroundColor = DTPoolBossInspectorUtility.BrightButtonColor;
                        var selCatIndex = catNames.IndexOf(poolItem.categoryName);
                        var newCat = EditorGUILayout.Popup(selCatIndex, catNames.ToArray(), GUILayout.Width(130));
                        if (newCat != selCatIndex)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ChangePoolItemCategory);
                            poolItem.categoryName = catNames[newCat];
                        }

                        GUI.backgroundColor = Color.white;

                        switch (poolItem.prefabSource)
                        {
                            case PoolBoss.PrefabSource.Prefab:
                                DTPoolBossInspectorUtility.FocusInProjectViewButton(PoolBossLang.PoolItemPrefab,
                                    poolItem.prefabTransform == null ? null : poolItem.prefabTransform.gameObject);
                                break;
#if ADDRESSABLES_ENABLED
                            case PoolBoss.PrefabSource.Addressable:
                                DTPoolBossInspectorUtility.FocusAddressableInProjectViewButton(PoolBossLang.PoolItemPrefab, poolItem.prefabAddressable);
                                break;
#endif
                        }
                    }

                    var buttonPressed = DTPoolBossInspectorUtility.AddFoldOutListItemButtons(i, matchingItems.Count,
                        PoolBossLang.PoolItem, false, null, true, false, true);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();

                    if (poolItem.isExpanded)
                    {
                        EditorGUI.indentLevel = 0;

#if ADDRESSABLES_ENABLED
                        var newSource =
 (PoolBoss.PrefabSource)EditorGUILayout.EnumPopup("Prefab Source", poolItem.prefabSource);
                        if (newSource != poolItem.prefabSource)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, "change Prefab Source");
                            poolItem.prefabSource = newSource;

                            if (poolItem.prefabSource == PoolBoss.PrefabSource.Addressable)
                            {
                                poolItem.prefabTransform = null; // clear it out to eliminate references
                            }
                        }
#endif

                        switch (poolItem.prefabSource)
                        {
                            case PoolBoss.PrefabSource.Prefab:
                                var newPrefab = (Transform)EditorGUILayout.ObjectField(PoolBossLang.Prefab, poolItem.prefabTransform, typeof(Transform), false);
                                if (newPrefab != poolItem.prefabTransform)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ChangePoolItemPrefab);
                                    poolItem.prefabTransform = newPrefab;
                                }
                                break;
#if ADDRESSABLES_ENABLED
                            case PoolBoss.PrefabSource.Addressable:
                                var itemNumber = _pool.poolItems.FindIndex(delegate (PoolBossItem item)
                                {
                                    return item == poolItem;
                                });

                                serializedObject.Update();

                                var poolItemsProp = serializedObject.FindProperty(nameof(PoolBoss.poolItems));
                                var poolItemProp =
 poolItemsProp.GetArrayElementAtIndex(itemNumber).FindPropertyRelative(nameof(PoolBossItem.prefabAddressable));

                                EditorGUILayout.PropertyField(poolItemProp, new GUIContent(PoolBossLang.PrefabAddressable, PoolBossLang.PrefabAddressableTooltip), true);

                                serializedObject.ApplyModifiedProperties();
                                break;
#endif
                        }

                        var newPreloadQty = EditorGUILayout.IntSlider(PoolBossLang.PreloadQty, poolItem.instancesToPreload, 0, 10000);
                        if (newPreloadQty != poolItem.instancesToPreload)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ChangePreloadQty);
                            poolItem.instancesToPreload = newPreloadQty;
                        }

                        if (poolItem.instancesToPreload == 0)
                        {
                            DTPoolBossInspectorUtility.ShowColorWarning(PoolBossLang.PreloadQtyToZero);
                        }

                        var newAllow = EditorGUILayout.Toggle(PoolBossLang.AllowInstantiate, poolItem.allowInstantiateMore);
                        if (newAllow != poolItem.allowInstantiateMore)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ToggleAllowInstantiate);
                            poolItem.allowInstantiateMore = newAllow;
                        }

                        if (poolItem.allowInstantiateMore)
                        {
                            var newLimit = EditorGUILayout.IntSlider(PoolBossLang.ItemLimit, poolItem.itemHardLimit, poolItem.instancesToPreload, 10000);
                            if (newLimit != poolItem.itemHardLimit)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ChangeItemLimit);
                                poolItem.itemHardLimit = newLimit;
                            }
                        }
                        else
                        {
                            var newRecycle = EditorGUILayout.Toggle(PoolBossLang.RecycleOldest, poolItem.allowRecycle);
                            if (newRecycle != poolItem.allowRecycle)
                            {
                                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ToggleRecycleOldest);
                                poolItem.allowRecycle = newRecycle;
                            }
                        }

#if UNITY_5_5 || UNITY_5_6 || UNITY_2017_1_OR_NEWER
                        if (poolItem.prefabTransform != null)
                        {
                            var navMeshAgent = poolItem.prefabTransform.GetComponent<NavMeshAgent>();
                            if (navMeshAgent != null)
                            {
                                var content = new GUIContent(PoolBossLang.EnableNavMesh, PoolBossLang.EnableNavMeshTooltip);
                                var newAgent = EditorGUILayout.Toggle(content, poolItem.enableNavMeshAgentOnSpawn);
                                if (newAgent != poolItem.enableNavMeshAgentOnSpawn)
                                {
                                    UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ToggleEnableNavMesh);
                                    poolItem.enableNavMeshAgentOnSpawn = newAgent;
                                }

                                if (poolItem.enableNavMeshAgentOnSpawn)
                                {
                                    var newDelay = EditorGUILayout.IntSlider(PoolBossLang.NavMeshAgentDelay, poolItem.delayNavMeshEnableByFrames, 0, 200);
                                    if (newDelay != poolItem.delayNavMeshEnableByFrames)
                                    {
                                        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool,PoolBossLang.ChangeNavMeshAgentDelay);
                                        poolItem.delayNavMeshEnableByFrames = newDelay;
                                    }
                                }
                            }
                        }
#endif

                        newLog = EditorGUILayout.Toggle(PoolBossLang.LogMessages, poolItem.logMessages);
                        if (newLog != poolItem.logMessages)
                        {
                            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.LogMessagesToggle);
                            poolItem.logMessages = newLog;
                        }
                    }

                    switch (buttonPressed)
                    {
                        case DTPoolBossInspectorUtility.FunctionButtons.Remove:
                            itemToRemove = poolItem;
                            break;
                        case DTPoolBossInspectorUtility.FunctionButtons.Add:
                            indexToInsertAt = _pool.poolItems.IndexOf(poolItem);
                            selectedCategory = cat;
                            break;
                        case DTPoolBossInspectorUtility.FunctionButtons.DespawnAll:
                            PoolBoss.DespawnAllOfPrefab(poolItem.prefabTransform);
                            break;
                        case DTPoolBossInspectorUtility.FunctionButtons.Copy:
                            itemToClone = poolItem;
                            break;
                    }

                    EditorGUILayout.EndVertical();
                    DTPoolBossInspectorUtility.AddSpaceForNonU5();
                }

                if (matchingItems.Count > 0)
                {
                    DTPoolBossInspectorUtility.EndGroupedControls();
                    DTPoolBossInspectorUtility.VerticalSpace(2);
                }
            }

            DTPoolBossInspectorUtility.VerticalSpace(2);
        }

        if (indexToShiftUp.HasValue)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ShiftUp);
            var item = _pool._categories[indexToShiftUp.Value];
            _pool._categories.Insert(indexToShiftUp.Value - 1, item);
            _pool._categories.RemoveAt(indexToShiftUp.Value + 1);
            _isDirty = true;
        }

        if (indexToShiftDown.HasValue)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ShiftDown);
            var index = indexToShiftDown.Value + 1;
            var item = _pool._categories[index];
            _pool._categories.Insert(index - 1, item);
            _pool._categories.RemoveAt(index + 1);
            _isDirty = true;
        }

        if (catToDelete != null)
        {
            if (_pool.poolItems.FindAll(delegate(PoolBossItem x) { return x.categoryName == catToDelete.CatName; }).Count > 0)
            {
                DTPoolBossInspectorUtility.ShowAlert(PoolBossLang.DeleteCategoryWithPool);
            }
            else if (_pool._categories.Count <= 1)
            {
                DTPoolBossInspectorUtility.ShowAlert(PoolBossLang.DeleteLastCategory);
            }
            else
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.DeleteCategory);
                _pool._categories.Remove(catToDelete);
                _isDirty = true;
            }
        }

        if (catRenaming != null)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            var isValidName = true;

            if (string.IsNullOrEmpty(catRenaming.ProspectiveName))
            {
                isValidName = false;
                DTPoolBossInspectorUtility.ShowAlert(PoolBossLang.BlankCategoryName);
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _pool._categories.Count; c++)
            {
                var cat = _pool._categories[c];
                // ReSharper disable once InvertIf
                if (cat != catRenaming && cat.CatName == catRenaming.ProspectiveName)
                {
                    isValidName = false;
                    var txt = string.Format(PoolBossLang.CategoryNameUnique, catRenaming.ProspectiveName);
                    DTPoolBossInspectorUtility.ShowAlert(txt);
                }
            }

            if (isValidName)
            {
                UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.UndoChangeCategory);

                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < _pool.poolItems.Count; i++)
                {
                    var item = _pool.poolItems[i];
                    if (item.categoryName == catRenaming.CatName)
                    {
                        item.categoryName = catRenaming.ProspectiveName;
                    }
                }

                catRenaming.CatName = catRenaming.ProspectiveName;
                catRenaming.IsEditing = false;
                _isDirty = true;
            }
        }

        if (catEditing != null)
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var c = 0; c < _pool._categories.Count; c++)
            {
                var cat = _pool._categories[c];
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (catEditing == cat)
                {
                    cat.IsEditing = true;
                }
                else
                {
                    cat.IsEditing = false;
                }

                _isDirty = true;
            }
        }

        if (itemToRemove != null)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.RemovePoolItem);
            _pool.poolItems.Remove(itemToRemove);
        }

        if (itemToClone != null)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ClonePoolItem);
            var newItem = itemToClone.Clone();

            var oldIndex = _pool.poolItems.IndexOf(itemToClone);

            _pool.poolItems.Insert(oldIndex, newItem);
        }

        if (indexToInsertAt.HasValue)
        {
            UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.InsertPoolItem);
            _pool.poolItems.Insert(indexToInsertAt.Value, new PoolBossItem
            {
                categoryName = selectedCategory.CatName
            });
        }

        if (GUI.changed || _isDirty)
        {
            EditorUtility.SetDirty(target); // or it won't save the data!!
        }

        //DrawDefaultInspector();
    }

    private void ExpandCollapseAll(bool isExpand)
    {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ToggleItems);

        foreach (var cat in _pool._categories)
        {
            cat.IsExpanded = isExpand;
        }

        foreach (var item in _pool.poolItems)
        {
            item.isExpanded = isExpand;
        }
    }

    private void ExpandCollapseCategory(string category, bool isExpand)
    {
        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.ToggleItemsInCategory);

        foreach (var cat in _pool._categories)
        {
            if (cat.CatName != category)
            {
                continue;
            }

            cat.IsExpanded = isExpand;
        }

        foreach (var item in _pool.poolItems)
        {
            if (item.categoryName != category)
            {
                continue;
            }

            item.isExpanded = isExpand;
        }
    }

    private void AddPoolItem(Object o)
    {
        // ReSharper disable once PossibleNullReferenceException
        var go = (o as GameObject);
        if (go == null)
        {
            DTPoolBossInspectorUtility.ShowAlert(PoolBossLang.DraggedNotGameObject);
            return;
        }

        var newItem = new PoolBossItem
        {
            categoryName = _pool.addToCategoryName,
            prefabSource = _pool.newItemPrefabSource
        };

        switch (_pool.newItemPrefabSource)
        {
            case PoolBoss.PrefabSource.Prefab:
                newItem.prefabTransform = go.transform;
                break;
#if ADDRESSABLES_ENABLED
            case PoolBoss.PrefabSource.Addressable:
                newItem.prefabAddressable =
 PoolBossAddressableEditorHelper.CreateAssetReferenceFromObject(go.transform);
                break;
#endif
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.AddPoolIem);

        _pool.poolItems.Add(newItem);
    }

    private void ClearAllPeaks()
    {
        for (var i = 0; i < _pool.poolItems.Count; i++)
        {
            var poolItem = _pool.poolItems[i].prefabTransform;
            if (poolItem == null)
            {
                continue;
            }

            var item = PoolBoss.PoolItemInfoByName(poolItem.name);
            item.Peak = Math.Max(0, item.SpawnedClones.Count);
            item.PeakTime = Time.realtimeSinceStartup;
        }

        _isDirty = true;
        _pool._changes++;
    }

    private void CreateCategory()
    {
        if (string.IsNullOrEmpty(_pool.newCategoryName))
        {
            DTPoolBossInspectorUtility.ShowAlert(PoolBossLang.ErrorBlankCategoryName);
            return;
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (var c = 0; c < _pool._categories.Count; c++)
        {
            var cat = _pool._categories[c];
            // ReSharper disable once InvertIf
            if (cat.CatName == _pool.newCategoryName)
            {
                DTPoolBossInspectorUtility.ShowAlert(string.Format(PoolBossLang.ErrorDuplicateCategoryName, _pool.newCategoryName));
                return;
            }
        }

        UndoHelper.RecordObjectPropertyForUndo(ref _isDirty, _pool, PoolBossLang.CreateNewCategory);

        var newCat = new PoolBossCategory
        {
            CatName = _pool.newCategoryName,
            ProspectiveName = _pool.newCategoryName,
        };

        _pool._categories.Add(newCat);
    }

    private string PoolBossItemName(PoolBossItem item)
    {
        switch (item.prefabSource)
        {
            case PoolBoss.PrefabSource.Prefab:
                return item.prefabTransform == null ? string.Empty : item.prefabTransform.name;
#if ADDRESSABLES_ENABLED
            case PoolBoss.PrefabSource.Addressable:
                return PoolBossAddressableEditorHelper.EditTimeAddressableName(item.prefabAddressable);
#endif
            default:
                throw new KeyNotFoundException(item.prefabSource.ToString());
        }
    }
}
