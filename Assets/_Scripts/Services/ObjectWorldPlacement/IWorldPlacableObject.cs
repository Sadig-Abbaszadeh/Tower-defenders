using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorldPlacableObject
{
    void OnCanPlace();

    void OnCanNotPlace();

    bool CanPlaceHere();

    void OnPlace();

    void OnPlaceFailed();

    Transform ThisTransform { get; }
}