using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using DartsGames.CUT.CoreExtensions;
using System;
using System.Reflection;
using DartsGames.CUT.EditorExtensions;
using DartsGames.CUT.UnityExtensions;
using DartsGames.Extensions;

[CustomPropertyDrawer(typeof(PolymorphList<>))]
public class ReorderableListEditor : PropertyDrawer
{
    private bool drawDefaultList = false;

    private bool initializedTypes = false;

    private (Type type, string fullPath)[] possibleTypes;

    private string nameContent;

    // reboot
    private Dictionary<string, ReorderableList> cachedLists = new Dictionary<string, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialize(property);

        //base.OnGUI(position, property, label);

        //reorderableList.DoList(position);

        cachedLists[property.propertyPath].DoList(position);

        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        Initialize(property);

        return cachedLists[property.propertyPath].GetHeight();
    }

    private void Initialize(SerializedProperty property)
    {
        // check unity object
        if (!initializedTypes)
        {
            InitializeTypes(property);
            initializedTypes = true;
        }

        if (!cachedLists.ContainsKey(property.propertyPath))
        {
            InitializeList(property);
        }
    }

    private void InitializeList(SerializedProperty property)
    {
        //Debug.Log("init prop drawer for " + property.propertyPath);
        var coll = property.FindPropertyRelative("collection");
        var collectionInfo = coll.GetFieldInformation();

        var value = collectionInfo.GetValue(property);

        var list = value as IList;

        var reorderableList = new ReorderableList(list, collectionInfo.PropertyType.GetGenericArguments()[0], true, true, true, true);
        reorderableList.serializedProperty = coll;


        reorderableList.drawHeaderCallback = DrawItemListHeader;
        reorderableList.drawElementBackgroundCallback = DrawItemListElementBackground;
        reorderableList.onRemoveCallback = OnItemListRemove;
        reorderableList.onSelectCallback = OnItemListSelect;
        reorderableList.onReorderCallback = OnItemListReorder;
        reorderableList.elementHeightCallback = (index) => OnItemHeight(reorderableList, index);
        cachedLists.Add(property.propertyPath, reorderableList);

        if (drawDefaultList)
        {
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => 
                DrawListElement_AsDefault(reorderableList, rect, index, isActive, isFocused);
            reorderableList.onAddCallback = Add_AsDefault;
        }
        else
        {
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) => 
                DrawListElement_AsPolymorph(reorderableList, rect, index, isActive, isFocused);
            reorderableList.onAddDropdownCallback = AddDropdown_AsPolymorph;
        }
    }

    private void InitializeTypes(SerializedProperty property)
    {
        var coll = property.FindPropertyRelative("collection");
        var collectionInfo = coll.GetFieldInformation();

        // get list type
        var listType = collectionInfo.PropertyType.GetGenericArguments()[0];

        // is unity object list
        if (typeof(UnityEngine.Object).IsAssignableFrom(listType))
        {
            nameContent = property.name;
            drawDefaultList = true;
            return;
        }

        // get banned types
        var bannedAttr = this.fieldInfo.GetCustomAttribute<BannedTypesAttribute>();

        var bannedTypes = bannedAttr != null ?
            bannedAttr.BannedTypes :
            new Type[0];

        nameContent = $"{property.name} : {listType.Name}";

        CacheTypes(listType, bannedTypes);
    }

    private void CacheTypes(Type baseType, IEnumerable<Type> bannedTypes)
    {
        possibleTypes = baseType.GetInheritingTypesWithPaths(bannedTypes: bannedTypes);
    }

    private float OnItemHeight(ReorderableList list, int index)
    {
        var prop = list.serializedProperty.GetArrayElementAtIndex(index);

        return EditorGUI.GetPropertyHeight(prop);
    }

    private void OnItemListReorder(ReorderableList list)
    {
        //
    }

    private void OnItemListSelect(ReorderableList list)
    {
        //
    }

    private void OnItemListRemove(ReorderableList list)
    {
        ReorderableList.defaultBehaviours.DoRemoveButton(list);
    }

    private void Add_AsDefault(ReorderableList list)
    {
        list.list.Add(null);
    }

    private void AddDropdown_AsPolymorph(Rect buttonRect, ReorderableList list)
    {
        var menu = new GenericMenu();

        foreach (var t in possibleTypes)
        {
            menu.AddItem(new GUIContent(t.fullPath), false, () => OnAdd(list, t.type));
        }

        menu.ShowAsContext();
    }

    private void OnAdd(ReorderableList list, Type t)
    {
        if (t.TryCreateDefaultInstance(out var newObj))
        {
            list.list.Add(newObj);
        }
        else
        {
            Debug.LogError("Can not create instance for type " + t +
                " in polymorph list because it has no constructor with 0 parameters");
        }
    }

    private void DrawItemListElementBackground(Rect rect, int index, bool isActive, bool isFocused)
    {
        ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, true);
    }

    private void DrawListElement_AsDefault(ReorderableList list, Rect rect, int index, bool isActive, bool isFocused)
    {
        var prop = list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(rect, prop, new GUIContent(prop.displayName), true);
    }

    private void DrawListElement_AsPolymorph(ReorderableList list, Rect rect, int index, bool isActive, bool isFocused)
    {
        var prop = list.serializedProperty.GetArrayElementAtIndex(index);

        rect.x = rect.x + 7;
        rect.width = rect.width - 7;
        var len = 17;
        EditorGUI.PropertyField(rect, prop, new GUIContent($"{prop.displayName} : {prop.type.Substring(len, prop.type.Length - len - 1)}"), true);
    }

    private void DrawItemListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, nameContent);
    }
}