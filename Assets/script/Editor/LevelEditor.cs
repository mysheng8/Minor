using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreater))]
[ExecuteInEditMode]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelCreater creater = (LevelCreater)target;
        if (GUILayout.Button("Export Level Desc Xml", GUILayout.Width(300)))
        {
            creater.ExportLevelDesc(creater.fileName);
        }
        if (GUILayout.Button("Export HeightMap",GUILayout.Width(300)))
        {

            //creater.StartCoroutine(creater.ExportHeightMap(creater.heightmap));
            creater.ExportHeightMap(creater.heightmap);
        }
    }
    void OnSceneGUI()
    {
        LevelCreater creater = (LevelCreater)target;


        float CellSizeX = creater.SpaceSize.x / creater.NumCell.x;
        float CellSizeY = creater.SpaceSize.y / creater.NumCell.y;


        float sy = creater.StartPos.y;
        for (int y = 0; y < creater.NumCell.y; ++y)
        {
            float sx = creater.StartPos.x;
            for (int x = 0; x < creater.NumCell.x; ++x)
            {
                Vector3 pos = new Vector3(sx + CellSizeX / 2, 0, sy + CellSizeY / 2);
                Vector3[] verts = { new Vector3(pos.x-CellSizeX / 2,pos.y,pos.z-CellSizeY / 2),
                                    new Vector3(pos.x-CellSizeX / 2,pos.y,pos.z+CellSizeY / 2),
                                    new Vector3(pos.x+CellSizeX / 2,pos.y,pos.z+CellSizeY / 2),
                                    new Vector3(pos.x+CellSizeX / 2,pos.y,pos.z-CellSizeY / 2)};
                Handles.DrawSolidRectangleWithOutline(verts, new UnityEngine.Color(1, 1, 1, 0.2f), new UnityEngine.Color(0, 0, 0, 1));
                sx += CellSizeX;
            }
            sy += CellSizeY;
        }
        
    }
}


[CustomEditor(typeof(GameWorld))]
[ExecuteInEditMode]
public class GameWorldEditor : Editor
{
    void OnSceneGUI()
    {
        GameWorld creater = (GameWorld)target;

        GameLevel level = creater.CurrentLevel;
        if (level == null)
            return;

        float CellSizeX = level.SpaceSize.x / level.NumCell.x;
        float CellSizeY = level.SpaceSize.y / level.NumCell.y;


        float sy = level.StartPos.y;
        for (int y = 0; y < level.NumCell.y; ++y)
        {
            float sx = level.StartPos.x;
            for (int x = 0; x < level.NumCell.x; ++x)
            {
                Vector3 pos = new Vector3(sx + CellSizeX / 2, 0, sy + CellSizeY / 2);
                Vector3[] verts = { new Vector3(pos.x-CellSizeX / 2,pos.y,pos.z-CellSizeY / 2),
                                    new Vector3(pos.x-CellSizeX / 2,pos.y,pos.z+CellSizeY / 2),
                                    new Vector3(pos.x+CellSizeX / 2,pos.y,pos.z+CellSizeY / 2),
                                    new Vector3(pos.x+CellSizeX / 2,pos.y,pos.z-CellSizeY / 2)};
                Handles.DrawSolidRectangleWithOutline(verts, new UnityEngine.Color(1, 1, 1, 0.2f), new UnityEngine.Color(0, 0, 0, 1));
                sx += CellSizeX;
            }
            sy += CellSizeY;
        }

    }
}
