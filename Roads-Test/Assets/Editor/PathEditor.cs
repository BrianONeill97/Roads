using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Create New"))
        {
            Undo.RecordObject(creator, "Create New");
            creator.CreatePath();
            path = creator.path;
        }

        if (GUILayout.Button("Toggle Closed"))
        {
            Undo.RecordObject(creator, "Toggle Closed");
            path.toggleOpen();
        }

        bool autoSetControlPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
        if(autoSetControlPoints != path.AutoSetControlPoints)
        {
            Undo.RecordObject(creator, "Toggle Auto Set Controls");
            path.AutoSetControlPoints = autoSetControlPoints;
        }

        if(EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }

    }

    void OnSceneGUI()
    {
        Input();
        draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector3 mousePosition = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

        if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(creator, " Add Segment");
            path.addSegment(new Vector3(mousePosition.x,0,mousePosition.z));
        }
    }

    void draw()
    {
        for(int i = 0; i < path.NumSegments; i++)
        {
            Vector3[] points = path.GetPointsInSegment(i);

            Handles.color = Color.black;
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);

            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
        }
        Handles.color = Color.red;
        for(int i = 0; i < path.NumPoints; i++)
        {
            Vector3 newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, .1f, Vector3.zero, Handles.CylinderHandleCap);
            if(path[i] != newPos)
            {
                Undo.RecordObject(creator, "Move Point");
                path.movePoint(i, new Vector3(newPos.x, 0, newPos.z));
            }
        }
    }

    void OnEnable()
    {
        creator = (PathCreator)target;
        if(creator.path == null)
        {
            creator.CreatePath();
        }
        path = creator.path;
    }
}
