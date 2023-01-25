using DartsGames.CUT.Attributes;
using DartsGames.CUT.UnityExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExtendEditor]
public class Test : MonoBehaviour
{
    [SerializeField]
    private bool auto = false;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private LineRenderer lineRenderer;

    private void Update()
    {
        if (auto || Input.GetKeyDown(KeyCode.U))
            GetPath();
    }

    private void GetPath()
    {
        var path = Astar2DController.Instance.FindPath(transform.position, target.position);

        lineRenderer.positionCount = path.Count;
        lineRenderer.SetPositions(path.Select(p => p.worldPosition).ToArray());
    }
}