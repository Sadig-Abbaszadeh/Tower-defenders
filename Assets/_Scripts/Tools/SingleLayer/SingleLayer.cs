using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SingleLayer
{
    [SerializeField, HideInInspector]
    private int _layer;

    public static implicit operator int(SingleLayer layer) => layer._layer;
    public static implicit operator SingleLayer(int layer) => new SingleLayer() { _layer = layer };

    public static implicit operator LayerMask(SingleLayer layer) => layer._layer;
    public static implicit operator SingleLayer(LayerMask mask) => new SingleLayer() { _layer = mask };

    public override string ToString() => _layer.ToString();

    public int BuiltInLayerIndex
    {
        get
        {
            if (_layer <= 0)
                return 0;

            return (int)Math.Log(_layer, 2);
        }
    }
}