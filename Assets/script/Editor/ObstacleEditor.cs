using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(StaticCircleObstacle))]
[ExecuteInEditMode]
public class StaticCircleObstacleEditor : Editor
{
    void OnSceneGUI()
    {
        StaticCircleObstacle obstacle = (StaticCircleObstacle)target;

        Handles.Label(obstacle.transform.position + Vector3.up * 2,
           obstacle.transform.name + " : " + obstacle.transform.position.ToString());
        Handles.DrawWireDisc(obstacle.transform.position, Vector3.up, obstacle.BRadius);
        float radius = 0;
        EditorGUI.BeginChangeCheck();
        radius = Handles.ScaleValueHandle(obstacle.BRadius, obstacle.transform.position + new Vector3(obstacle.BRadius, 0, 0), Quaternion.identity, 20, Handles.CubeCap, 1);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(obstacle, "Change BRadius");
            obstacle.BRadius = radius;
            EditorUtility.SetDirty(target);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(obstacle);
    }
}

[CustomEditor(typeof(StaticRectObstacle))]
[ExecuteInEditMode]
public class StaticRectObstacleEditor : Editor
{
    void OnSceneGUI()
    {
        StaticRectObstacle obstacle = (StaticRectObstacle)target;

        Handles.Label(obstacle.transform.position + Vector3.up * 2,
           obstacle.transform.name + " : " + obstacle.transform.position.ToString());
        Handles.DrawWireDisc(obstacle.transform.position, Vector3.up, obstacle.BRadius);
        float radius = 0;
        EditorGUI.BeginChangeCheck();
        radius = Handles.ScaleValueHandle(obstacle.BRadius, obstacle.transform.position + new Vector3(obstacle.BRadius, 0, 0), Quaternion.identity, 20, Handles.CubeCap, 1);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(obstacle, "Change BRadius");
            obstacle.BRadius = radius;
            EditorUtility.SetDirty(target);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(obstacle);
        float height = obstacle.transform.position.y;
        Vector2 position = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
        Vector2 heading = new Vector2(obstacle.transform.forward.x, obstacle.transform.forward.z);
        Vector2 side = new Vector2(heading.y, -heading.x);

        Vector2[] corners = { yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMin,obstacle.m_Region.yMin), heading, side, position),
                              yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMin,obstacle.m_Region.yMax), heading, side, position),
                              yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMax,obstacle.m_Region.yMax), heading, side, position),
                              yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMax,obstacle.m_Region.yMin), heading, side, position)};
        Vector3[] verts = { new Vector3(corners[0].x, height, corners[0].y),
                          new Vector3(corners[1].x, height, corners[1].y),
                          new Vector3(corners[2].x, height, corners[2].y),
                          new Vector3(corners[3].x, height, corners[3].y)};

        Handles.DrawSolidRectangleWithOutline(verts, new UnityEngine.Color(1, 1, 1, 0.2f), new UnityEngine.Color(0, 0, 0, 1));

    }
}

[CustomEditor(typeof(FollowPathCircleObstacle))]
[ExecuteInEditMode]
public class FollowPathCircleObstacleEditor : Editor
{

    bool PathEditMode = true;
    public void AddPathPointHelper()
    {
        FollowPathCircleObstacle obstacle = (FollowPathCircleObstacle)target;
        obstacle.m_Data.PointList.Add(obstacle.transform.position);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FollowPathCircleObstacle obstacle = (FollowPathCircleObstacle)target;
        
        if (GUILayout.Button("Add Path Point Helper", GUILayout.Width(300)))
        {
            AddPathPointHelper();
        }
        PathEditMode = EditorGUILayout.Toggle("Path Point Edit Mode", PathEditMode);
    }

    void OnSceneGUI()
    {
        FollowPathCircleObstacle obstacle = (FollowPathCircleObstacle)target;

        Handles.Label(obstacle.transform.position + Vector3.up * 2,
           obstacle.transform.name + " : " + obstacle.transform.position.ToString());

        

        if (PathEditMode)
        {
            if (obstacle.m_Data.PointList.Count > 0)
            {
                
                EditorGUI.BeginChangeCheck();
                Vector3 pointPos = Vector3.zero;
                Vector3 newPos = Vector3.zero;
                int index = 0;
                for (int i = 0; i != obstacle.m_Data.PointList.Count; ++i)
                {
                    pointPos = Handles.PositionHandle(obstacle.m_Data.PointList[i], Quaternion.identity);
                    if (pointPos != obstacle.m_Data.PointList[i])
                    {
                        index = i;
                        newPos = pointPos;
                    }
                    Handles.SphereHandleCap(0, pointPos, Quaternion.identity, 10, EventType.Repaint);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(obstacle, "Change Path Point");
                    obstacle.m_Data.PointList[index] = newPos;

                    EditorUtility.SetDirty(target);
                }
                Handles.DrawAAPolyLine(obstacle.m_Data.PointList.ToArray());

            }
        }
        if (GUI.changed)
            EditorUtility.SetDirty(obstacle);
        Handles.DrawWireDisc(obstacle.transform.position, Vector3.up, obstacle.BRadius);
        float radius = 0;
        EditorGUI.BeginChangeCheck();
        radius = Handles.ScaleValueHandle(obstacle.BRadius, obstacle.transform.position + new Vector3(obstacle.BRadius, 0, 0), Quaternion.identity, 20, Handles.CubeCap, 1);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(obstacle, "Change BRadius");
            obstacle.BRadius = radius;
            EditorUtility.SetDirty(target);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(obstacle);
    }
}


[CustomEditor(typeof(FollowPathRectObstacle))]
[ExecuteInEditMode]
public class FollowPathRectObstacleEditor : Editor
{

    bool PathEditMode = true;
    public void AddPathPointHelper()
    {
        FollowPathRectObstacle obstacle = (FollowPathRectObstacle)target;
        obstacle.m_Data.PointList.Add(obstacle.transform.position);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FollowPathRectObstacle obstacle = (FollowPathRectObstacle)target;

        if (GUILayout.Button("Add Path Point Helper", GUILayout.Width(300)))
        {
            AddPathPointHelper();
        }
        PathEditMode = EditorGUILayout.Toggle("Path Point Edit Mode", PathEditMode);
    }

    void OnSceneGUI()
    {
        FollowPathRectObstacle obstacle = (FollowPathRectObstacle)target;

        Handles.Label(obstacle.transform.position + Vector3.up * 2,
           obstacle.transform.name + " : " + obstacle.transform.position.ToString());



        if (PathEditMode)
        {
            if (obstacle.m_Data.PointList.Count > 0)
            {

                EditorGUI.BeginChangeCheck();
                Vector3 pointPos = Vector3.zero;
                Vector3 newPos = Vector3.zero;
                int index = 0;
                for (int i = 0; i != obstacle.m_Data.PointList.Count; ++i)
                {
                    pointPos = Handles.PositionHandle(obstacle.m_Data.PointList[i], Quaternion.identity);
                    if (pointPos != obstacle.m_Data.PointList[i])
                    {
                        index = i;
                        newPos = pointPos;
                    }
                    Handles.SphereHandleCap(0, pointPos, Quaternion.identity, 10, EventType.Repaint);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(obstacle, "Change Path Point");
                    obstacle.m_Data.PointList[index] = newPos;

                    EditorUtility.SetDirty(target);
                }
                Handles.DrawAAPolyLine(obstacle.m_Data.PointList.ToArray());

            }
        }
        if (GUI.changed)
            EditorUtility.SetDirty(obstacle);
        Handles.DrawWireDisc(obstacle.transform.position, Vector3.up, obstacle.BRadius);
        float radius = 0;
        EditorGUI.BeginChangeCheck();
        radius = Handles.ScaleValueHandle(obstacle.BRadius, obstacle.transform.position + new Vector3(obstacle.BRadius, 0, 0), Quaternion.identity, 20, Handles.CubeCap, 1);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(obstacle, "Change BRadius");
            obstacle.BRadius = radius;
            EditorUtility.SetDirty(target);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(obstacle);

        float height = obstacle.transform.position.y;
        Vector2 position = new Vector2(obstacle.transform.position.x, obstacle.transform.position.z);
        Vector2 heading = new Vector2(obstacle.transform.forward.x,obstacle.transform.forward.z);
        Vector2 side = new Vector2(heading.y, -heading.x);

        Vector2[] corners = { yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMin,obstacle.m_Region.yMin), heading, side, position),
                              yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMin,obstacle.m_Region.yMax), heading, side, position),
                              yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMax,obstacle.m_Region.yMax), heading, side, position),
                              yMath.PointToWorldSpace(new Vector2(obstacle.m_Region.xMax,obstacle.m_Region.yMin), heading, side, position)};
        Vector3[] verts = { new Vector3(corners[0].x, height, corners[0].y),
                          new Vector3(corners[1].x, height, corners[1].y),
                          new Vector3(corners[2].x, height, corners[2].y),
                          new Vector3(corners[3].x, height, corners[3].y)};

        Handles.DrawSolidRectangleWithOutline(verts, new UnityEngine.Color(1, 1, 1, 0.2f), new UnityEngine.Color(0, 0, 0, 1));
    }
}
