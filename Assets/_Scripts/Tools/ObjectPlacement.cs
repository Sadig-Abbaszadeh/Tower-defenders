using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    public Transform testObj;
    public LayerMask mask;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            TestRay();
    }

    private void TestRay()
    {
        var cam = Camera.main;
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, 50f, mask))
            return;

        testObj.transform.position = hit.point;
    }
}