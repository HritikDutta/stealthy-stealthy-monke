using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DemonPathEditorWindow : EditorWindow
{
    [MenuItem("Tools/Demon Path Editor")]
    public static void Open()
    {
        GetWindow<DemonPathEditorWindow>();
    }

    public Transform pathParentTransform;

    void OnGUI()
    {
        SerializedObject obj = new SerializedObject(this);

        EditorGUILayout.PropertyField(obj.FindProperty("pathParentTransform"));

        EditorGUILayout.BeginVertical();
        DrawButtons();
        EditorGUILayout.EndVertical();

        obj.ApplyModifiedProperties();
    }

    void DrawButtons()
    {
        if (GUILayout.Button("Add Waypoint"))
        {
            // Add a waypoint to the path
            GameObject waypointObject = new GameObject("Waypoint " + pathParentTransform.childCount);
            waypointObject.transform.SetParent(pathParentTransform, true);
            Selection.activeGameObject = waypointObject;
        }

        if (GUILayout.Button("Remove Waypoint"))
        {
            GameObject selectedWaypoint = Selection.activeGameObject;
            if (selectedWaypoint != null)
                DestroyImmediate(selectedWaypoint);
        }
    }
}
