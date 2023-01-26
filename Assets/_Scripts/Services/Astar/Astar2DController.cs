using DartsGames.CUT.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DartsGames.CUT.UnityExtensions;
using DartsGames.CUT.MathExtensions;

[ExtendEditor]
public class Astar2DController : MonoBehaviour
{
    #region Editor
#if UNITY_EDITOR
    [UnityEditor.MenuItem("DartsGames/CUT/Add Astar2D Controller")]
    private static void AddAstar()
    {
        if (Instance)
            goto Show;

        var controller = FindObjectOfType<Astar2DController>();

        if (!controller)
            controller = GenerateController();

        Instance = controller;

    Show:
        UnityEditor.Selection.activeObject = Instance;
    }

    private static Astar2DController GenerateController()
    {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.localScale = new Vector3(5f, .1f, 5f);
        obj.name = "Astar controller";
        DestroyImmediate(obj.GetComponent<Collider>());

        return obj.AddComponent<Astar2DController>();
    }
#endif
    #endregion

    public static Astar2DController Instance { get; private set; }

    [SerializeField]
    private Astar2D astar;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        astar.BakeNavigationArea(transform);
    }

    private void OnValidate()
    {
        transform.rotation = Quaternion.identity;
    }

    public List<Vector3> FindPath(Vector3 position, Vector3 target) => astar.FindPath(position, target);

    #region Test
    [InspectorButton]
    private void Bake() => astar.BakeNavigationArea(transform);

    public void SetIntersect(Vector3 pos, float rad)
    {
        astar.NodesTouchingCircle(pos, rad, node => node.inCircle = true);
    }

    [Space(10), SerializeField]
    private bool showNodes = true;

    private void OnDrawGizmos()
    {
        if (!showNodes) return;

        astar.DrawNodes();
    }
    #endregion
}