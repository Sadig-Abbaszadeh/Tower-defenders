using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DartsGames.CUT.UnityExtensions;

public class ObjectPlacement : MonoBehaviour
{
    [SerializeField]
    private LayerMask totalRaycastMask, outOfBoundsRaycastMask;

    //
    private Camera cam;

    private bool isPlacingObject = false;
    private bool canPlace = true;
    private IWorldPlacableObject currentPlacable;
    private Transform currentDraggingTransform;

    private void Start()
    {
        cam = Camera.main;
        // ensure
        totalRaycastMask |= outOfBoundsRaycastMask;
    }

    private void Update()
    {
        if (!isPlacingObject)
            return;

        if (Input.GetMouseButtonUp(0))
        {
            FinalizePlacing();
            return;
        }

        TempPlaceObject();
    }

    private void TempPlaceObject()
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        bool canPlaceHere;

        if (!Physics.Raycast(ray, out var hit, 30f, totalRaycastMask))
        {
            canPlaceHere = false;
        }
        else
        {
            canPlaceHere = outOfBoundsRaycastMask.ContainsLayer(hit.transform.gameObject.layer) ?
                false : currentPlacable.CanPlaceHere();

            currentDraggingTransform.position = hit.point;
        }

        if (canPlace != canPlaceHere)
        {
            if (canPlaceHere)
                currentPlacable.OnCanPlace();
            else
                currentPlacable.OnCanNotPlace();
        }

        canPlace = canPlaceHere;
    }

    private void FinalizePlacing()
    {
        if (currentPlacable.CanPlaceHere())
            currentPlacable.OnPlace();
        else
            currentPlacable.OnPlaceFailed();

        isPlacingObject = false;
    }

    public void StartPlacing(IWorldPlacableObject placableObject)
    {
        this.canPlace = true;
        this.isPlacingObject = true;
        this.currentPlacable = placableObject;
        this.currentDraggingTransform = placableObject.ThisTransform;
    }
}