using DartsGames.CUT.EditorExtensions;
using DartsGames.CUT.UnityExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SingleLayer))]
public class SingleLayerEditor : ResolvedPropertiesDrawer<SingleLayerEditor.LayerInfo>
{
    private int[] layerValues;
    private GUIContent[] layerNames;

    private bool setuplayers = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!setuplayers)
            SetupLayers();

        base.OnGUI(position, property, label);

        var layerInfo = base.propertyBindings[property.propertyPath];

        var currentIndex = layerInfo.currentSelected;
        var newIndex = EditorGUI.Popup(position, label, currentIndex, layerNames);

        if (currentIndex == newIndex)
            return;

        layerInfo.currentSelected = newIndex;
        var layerBitValue = 1 << layerValues[newIndex];

        layerInfo.reflectInfo.SetValue(property, (SingleLayer)layerBitValue);

        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!setuplayers)
            SetupLayers();

        return base.GetPropertyHeight(property, label);
    }

    protected override LayerInfo InitializeProperty(SerializedProperty prop)
    {
        var info = new LayerInfo();
        info.reflectInfo = prop.GetFieldInformation();

        int layerBitValue = (SingleLayer)info.reflectInfo.GetValue(prop);

        if (layerBitValue == 0)
        {
            info.reflectInfo.SetValue(prop, (SingleLayer)1);
            info.currentSelected = 0;
        }
        else
        {
            var layer = (int)Math.Log(layerBitValue, 2);
                
            info.currentSelected = LayerIndexInArray(layer);
        }

        return info;
    }

    private int LayerIndexInArray(int layer)
    {
        for(int i = 0; i < layerValues.Length; i++)
        {
            if (layerValues[i] == layer)
                return i;
        }

        return -1;
    }

    private void SetupLayers()
    {
        setuplayers = true;
        var _layers = new List<int>();
        var _names = new List<string>();

        for(int i = 0; i <= 31; i++)
        {
            var layerName = LayerMask.LayerToName(i);

            if (string.IsNullOrEmpty(layerName))
                continue;

            _layers.Add(i);
            _names.Add(layerName);
        }

        layerValues = _layers.ToArray();
        layerNames = _names.Select(n => new GUIContent(n)).ToArray();
    }

    public class LayerInfo
    {
        public SerializedPropertyReflect reflectInfo;
        public int currentSelected;
    }
}