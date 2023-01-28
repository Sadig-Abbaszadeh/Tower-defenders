using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Astar2DGrid
{
    [SerializeField, Min(.02f)]
    private float nodeSize = .5f;
    [SerializeField, Min(.2f)]
    private float nodeHeightCheck = 1f;

    private Vector3 gridCorner;

    private Arr2D<Vector3> points;

    public Astar2DGrid(Transform worldTransform)
    {
        worldTransform.rotation = Quaternion.identity;

        var scale = worldTransform.localScale;
        gridCorner = worldTransform.position - .5f * new Vector3(scale.x, 0, scale.z);

        var gridX = Mathf.FloorToInt(scale.x / nodeSize);
        var gridY = Mathf.FloorToInt(scale.z / nodeSize);

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                var vec3 = gridCorner + new Vector3(x * nodeSize + nodeSize * .5f, 0,
                    y * nodeSize + nodeSize * .5f);


            }
        }
    }
}