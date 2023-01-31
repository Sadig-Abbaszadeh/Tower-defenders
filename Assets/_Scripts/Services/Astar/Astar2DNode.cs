using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar2DNode
{
    public bool walkable;

    public int x, y;
    public int gCost, hCost;
    public int movePenalty;

    public Astar2DNode parent;

    public int fCost => gCost + hCost;

    public Astar2DNode GetCleanClone() => new Astar2DNode()
    {
        walkable = this.walkable,
        x = this.x,
        y = this.y,
        movePenalty = this.movePenalty,
    };

    // List lookup optimization?
    public override bool Equals(object obj)
    {
        var other = obj as Astar2DNode;

        return x == other.x && y == other.y;
    }

    public int GetDiagonalizedDistanceToOtherNode(Astar2DNode other)
    {
        var distX = Mathf.Abs(x - other.x);
        var distY = Mathf.Abs(y - other.y);

        if(distY < distX)
        {
            return distY * Astar2D.DiagonalCost +
                (distX - distY) * Astar2D.StraightCost;
        }
        else
        {
            return distX * Astar2D.DiagonalCost +
                (distY - distX) * Astar2D.StraightCost;
        }
    }

    public bool CostIsLowerThan(Astar2DNode other)
    {
        var fThis = fCost;
        var fOther = other.fCost;

        return fThis < fOther || (fThis == fOther && hCost <= other.hCost);
    }
}