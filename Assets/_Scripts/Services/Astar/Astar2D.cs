using DartsGames.CUT.CoreExtensions;
using DartsGames.CUT.UnityExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UColor = UnityEngine.Color;

[System.Serializable]
public class Astar2D
{
    private float nodeSize;
    private Vector3 gridCorner;

    private Arr2D<Astar2DNode> nodes;

    public Astar2D(float nodeSize, LayerMask nonWalkableLayers, Transform worldTransform,
        LayerMoveCost[] penaltyLayers = null, float nodeHeightCheck = 1f)
    {
        BakeNavigationArea(nodeSize, nonWalkableLayers, worldTransform, penaltyLayers, nodeHeightCheck);
    }

    public void BakeNavigationArea(float nodeSize, LayerMask nonWalkableLayers, Transform worldTransform,
        LayerMoveCost[] penaltyLayers = null, float nodeHeightCheck = 1f)
    {
        this.nodeSize = nodeSize;

        worldTransform.rotation = Quaternion.identity;

        var scale = worldTransform.localScale;
        gridCorner = worldTransform.position - .5f * new Vector3(scale.x, 0, scale.z);

        var gridX = Mathf.FloorToInt(scale.x / nodeSize);
        var gridY = Mathf.FloorToInt(scale.z / nodeSize);
        var nodeBoxExtents = new Vector3(nodeSize * .5f, nodeHeightCheck, nodeSize * .5f);

        nodes = new Arr2D<Astar2DNode>(gridX, gridY);

        var skipPenaltyChecks = penaltyLayers.IsNullOrEmpty();
        var walkableLayers = skipPenaltyChecks ? (LayerMask)0 : 
            WalkableLayers(penaltyLayers);
        var penaltyLookupTable = skipPenaltyChecks ? null :
            penaltyLayers.ToDictionary(l => l.layer.BuiltInLayerIndex, l => l.movePenalty);

        for (int x = 0; x < gridX; x++)
        {
            for (int y = 0; y < gridY; y++)
            {
                var node = new Astar2DNode();
                var worldPosition = GetWorldPosition(x, y);

                node.walkable = !Physics.CheckBox(worldPosition, nodeBoxExtents, Quaternion.identity, nonWalkableLayers);

                node.x = x;
                node.y = y;
                nodes[x, y] = node;

                if (skipPenaltyChecks)
                    continue;

                var ray = new Ray(worldPosition + Vector3.up * 50, Vector3.down);

                if (Physics.Raycast(ray, out var result, 100f, walkableLayers))
                    node.movePenalty = penaltyLookupTable[result.transform.gameObject.layer];
            }
        }
    }

    private LayerMask WalkableLayers(LayerMoveCost[] penaltyLayers)
    {
        LayerMask mask = 0;

        foreach (var layer in penaltyLayers)
            mask |= layer.layer;

        return mask;
    }

    #region Pathfinding

    public List<Vector3> FindPath(Vector3 position, Vector3 target)
    {
        var start = TryGetClosestNode(position);
        var end = TryGetClosestNode(target);

        if (start == null || end == null)
            return null;

        return FindPath(start, end, nodes);
    }

    private List<Vector3> FindPath(Astar2DNode startNode, Astar2DNode endNode, Arr2D<Astar2DNode> evaluationNodes)
    {
        // reset start and end
        startNode.gCost = 0;
        startNode.hCost = 0; // ??
        startNode.parent = null;

        var openList = new List<Astar2DNode>() { startNode };
        var closedList = new List<Astar2DNode>();

        // main loop
        while (true)
        {
            // get smallest f node
            var currentNode = GetLowestCostNode();

            if (currentNode.Equals(endNode))
                break;

            //  iterate neighbors
            foreach (var offset in neighborOffsets)
            {
                var neighborX = currentNode.x + offset.x;
                var neighborY = currentNode.y + offset.y;

                if (!evaluationNodes.IsValidCoordinate(neighborX, neighborY))
                    continue;

                var neighbor = evaluationNodes[neighborX, neighborY];

                if (!neighbor.walkable || closedList.Contains(neighbor))
                    continue;

                var neighborIsInOpen = openList.Contains(neighbor);
                var stepCost = offset.x == 0 || offset.y == 0 ? StraightCost
                    : DiagonalCost;
                var newGCost = currentNode.gCost + stepCost + neighbor.movePenalty;


                if (!neighborIsInOpen || newGCost <= neighbor.gCost)
                {
                    neighbor.gCost = newGCost;
                    neighbor.parent = currentNode;

                    if (!neighborIsInOpen)
                    {
                        neighbor.hCost = neighbor.GetDiagonalizedDistanceToOtherNode(endNode);
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return GetPathToNode(endNode);

        Astar2DNode GetLowestCostNode()
        {
            Astar2DNode lowestCostNode = openList[0];
            var _count = openList.Count;
            var ind = 0;

            for (int i = 1; i < _count; i++)
            {
                var newNode = openList[i];

                if (newNode.CostIsLowerThan(lowestCostNode))
                {
                    lowestCostNode = newNode;
                    ind = i;
                }
            }

            openList.RemoveAt(ind);
            closedList.Add(lowestCostNode);
            return lowestCostNode;
        }
    }

    private List<Vector3> GetPathToNode(Astar2DNode endNode)
    {
        var list = new List<Vector3>();

        var currentNode = endNode;

        while (currentNode != null)
        {
            list.Add(GetWorldPosition(currentNode.x, currentNode.y));
            currentNode = currentNode.parent;
        }

        // TODO REVERSE
        return list;
    }
    #endregion

    #region Async pathfinding
    public async Task<List<Vector3>> FindPathAsync(Vector3 position, Vector3 target)
    {
        var start = TryGetClosestNode(position);
        var end = TryGetClosestNode(target);

        if (start == null || end == null)
            return null;

        var result = await Task.Run(() => FindPath(start, end, GetEvaluationNodes()));
        return result;
    }
    #endregion

    #region Utils
    private static readonly Vector2Int[] neighborOffsets = new Vector2Int[]
    {
        new Vector2Int(-1, -1),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(0, 1),
        new Vector2Int(1, 1),
    };

    public const int DiagonalCost = 14, StraightCost = 10;

    public Astar2DNode TryGetClosestNode(Vector3 worldPosition)
    {
        if (nodes == null)
            return null;

        var relativePos = worldPosition - gridCorner;

        var x = Mathf.FloorToInt(relativePos.x / nodeSize);
        var y = Mathf.FloorToInt(relativePos.z / nodeSize);

        if (nodes.IsValidCoordinate(x, y))
            return nodes[x, y];

        return null;
    }

    public void NodesTouchingCircle(Vector3 circleWorldPos, float radius, Action<Astar2DNode> actionForNodes)
    {
        if (radius <= 0 || actionForNodes == null)
            return;

        var circPos = circleWorldPos.XZ();
        var sqTwo = Mathf.Sqrt(2f);
        var nodeHalf = .5f * nodeSize;

        foreach (var node in nodes)
        {
            var sqPos = GetWorldPosition(node.x, node.y).XZ();
            var dist = Vector2.Distance(circPos, sqPos);

            if (dist <= 0f)
            {
                actionForNodes(node);
                continue;
            }

            var sin2Alpha = 2 * (sqPos.x - circPos.x) * (sqPos.y - circPos.y) / (dist * dist);

            if (dist < radius + nodeHalf * (1 + (sqTwo - 1) * Mathf.Abs(sin2Alpha)))
                actionForNodes(node);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetWorldPosition(int x, int y)
    {
        return gridCorner + new Vector3(x * nodeSize + nodeSize * .5f, 0,
                    y * nodeSize + nodeSize * .5f);
    }

    private Arr2D<Astar2DNode> GetEvaluationNodes()
    {
        var newNodes = new Arr2D<Astar2DNode>(nodes.Width, nodes.Height);

        for (int x = 0; x < nodes.Width; x++)
            for (int y = 0; y < nodes.Height; y++)
                newNodes[x, y] = nodes[x, y].GetCleanClone();

        return newNodes;
    }
    #endregion

    #region Test
    public void DrawNodes()
    {
        UColor color;

        if (nodes == null)
            return;

        foreach (var node in nodes)
        {
            color = node.walkable ? UColor.white : UColor.red;

            Gizmos.color = color;
            Gizmos.DrawWireCube(GetWorldPosition(node.x, node.y), new Vector3(nodeSize, .25f, nodeSize) * 0.9f);
        }

        Gizmos.color = UColor.white;
    }
    #endregion
}