#define EDITORMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

[Serializable]
public class WallDesc
{
    public string assetPath;
    public Vector3 pos;
    public Quaternion rot;
}

[Serializable]
public class ObstacleDesc
{
    public string assetPath;
    public Vector3 pos;
    public Quaternion rot;
}


[Serializable]
public class LevelDesc
{
    public string assetPath;
    public Vector3 pos;
    public Quaternion rot;
    public Vector2 EnterPos;
    public Vector2 ExitPos;

    public Vector2 NumCell;
    public Vector2 SpaceSize;
    public Vector2 StartPos;

    public List<WallDesc> Walls;
    public List<ObstacleDesc> Obstacles;

} 




public class LevelCreater : MonoBehaviour {
    public string fileName;

    public Vector2 NumCell;
    public Vector2 SpaceSize;
    public Vector2 StartPos;

	// Use this for initialization
	void Start () {
        DisplaysCellSpacePartition();
	}

    void DisplaysCellSpacePartition()
    {
        float CellSizeX = SpaceSize.x / NumCell.x;
        float CellSizeY = SpaceSize.y / NumCell.y;

        
        float sy = StartPos.y;
        for (int y = 0; y < NumCell.y; ++y)
        {
            float sx = StartPos.x;
            for (int x = 0; x < NumCell.x; ++x)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(sx + CellSizeX / 2, 0, sy + CellSizeY / 2);
                cube.transform.localScale = new Vector3(CellSizeX, 1, CellSizeY);
                sx += CellSizeX;
            }
            sy += CellSizeY;
        }
    }

    LevelDesc GetLevelDescs()
    {
        LevelDesc ld = new LevelDesc();
        ld.Walls = new List<WallDesc>();
        ld.Obstacles = new List<ObstacleDesc>();
        Transform enterTF = GameObject.Find("EnterPos").transform;
        ld.EnterPos = new Vector2(enterTF.position.x, enterTF.position.z);
        Transform exitTF = GameObject.Find("ExitPos").transform;
        ld.ExitPos = new Vector2(exitTF.position.x, exitTF.position.z);
        ld.NumCell = NumCell;
        ld.SpaceSize = SpaceSize;
        ld.StartPos = StartPos;

        GameObject wallRoot = GameObject.Find("WallRoot");
        int wallsCount = wallRoot.transform.childCount;
        for (int i = 0; i < wallsCount; ++i)
        {
            Transform wallTF = wallRoot.transform.GetChild(i);
            WallDesc wd = new WallDesc();
            wd.assetPath = GetResourcePath(AssetDatabase.GetAssetPath(EditorUtility.GetPrefabParent(wallTF.gameObject)));
            wd.pos = wallTF.position;
            wd.rot = wallTF.rotation;

            ld.Walls.Add(wd);
        }

        GameObject obstacleRoot = GameObject.Find("ObstacleRoot");
        int obstacleCount = obstacleRoot.transform.childCount;
        for (int j = 0; j < obstacleCount; ++j)
        {
            Transform obstacleTF = obstacleRoot.transform.GetChild(j);
            ObstacleDesc od = new ObstacleDesc();
            od.assetPath = GetResourcePath(AssetDatabase.GetAssetPath(EditorUtility.GetPrefabParent(obstacleTF.gameObject)));
            od.pos = obstacleTF.position;
            od.rot = obstacleTF.rotation;

            ld.Obstacles.Add(od);
        }

        GameObject map = GameObject.Find("Map");
        ld.assetPath = GetResourcePath(AssetDatabase.GetAssetPath(EditorUtility.GetPrefabParent(map)));
        ld.pos = map.transform.position;
        ld.rot = map.transform.rotation;
        return ld;
    }

    string GetResourcePath(string filepath)
    {
        string name = System.IO.Path.GetFileNameWithoutExtension(filepath);
        string[] folders = filepath.Trim(Path.AltDirectorySeparatorChar).Split(Path.AltDirectorySeparatorChar);
        Debug.Log(folders.Length);
        string combinePath = "";
        int resourceIndex = folders.Length;
        for (int i = 0; i < folders.Length - 1; ++i)
        {
            if (folders[i] == "resources")
            {
                resourceIndex = i+1;
            }
            if (i == resourceIndex)
            {
                combinePath += folders[i] + Path.AltDirectorySeparatorChar;
                resourceIndex = i + 1;
            }
        }
        if (combinePath != "")
        {
            combinePath += name;
        }

        return combinePath;
    }

    void ExportLevelDesc(string fileName)
    {
        LevelDesc ld = GetLevelDescs();
        string dataFilePath = Application.streamingAssetsPath + "/" + fileName;
        string dataString = XmlUtils.SerializeObject(ld, typeof(LevelDesc));
        XmlUtils.CreateXML(dataFilePath, dataString);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 50), "Export Level Desc Xml"))
        {
            ExportLevelDesc(fileName);
        }

    }  
}
