using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// [InitializeOnLoad()]
public class DemonPathEditor
{
    // [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    // public static void OnDrawGizmos(PathComponent pathObject, GizmoType gizmoType)
    // {
        // for (int i = 0; i < pathObject.transform.childCount; i++)
        // {
        //     Transform point = pathObject.transform.GetChild(i);

        //     Gizmos.color = Color.blue;

        //     if (Selection.Contains(point.gameObject))
        //         Gizmos.color = Color.green;

        //     Gizmos.DrawSphere(point.position, 0.1f);

        //     Gizmos.color = Color.red;

        //     int prevIndex = (i + pathObject.transform.childCount - 1) % pathObject.transform.childCount;
        //     Gizmos.DrawLine(point.position, pathObject.transform.GetChild(prevIndex).position);
        // }
    // }
}
