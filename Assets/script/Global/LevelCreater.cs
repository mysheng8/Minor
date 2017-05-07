
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
public class MapDesc
{
    public string assetPath;
    public Vector3 pos;
    public Quaternion rot;
}



[Serializable]
public class LevelDesc
{
    public Vector2 EnterPos;
    public Vector2 ExitPos;

    public Vector2 NumCell;
    public Vector2 SpaceSize;
    public Vector2 StartPos;

    public List<WallDesc> Walls;
    public List<ObstacleDesc> Obstacles;
    public List<MapDesc> MapObjects;

    public string Heightmap;
    public Vector2 heightmapUVOffset;
    public Rect heightmapSampleRect;
} 




public class LevelCreater : MonoBehaviour {
    public string fileName;

    public Vector2 NumCell;
    public Vector2 SpaceSize;
    public Vector2 StartPos;

    public string heightmap;
    public Vector2 heightmapUVOffset;
    public Rect heightmapSampleRect;

    List<GameObject> cubelist;

	// Use this for initialization

	void Start () {
        cubelist = new List<GameObject>();
	}
    
    void DisplaysCellSpacePartition()
    {
        if (cubelist.Count > 0)
        {
            foreach (GameObject o in cubelist)
            {
                UnityEngine.Object.Destroy(o);
            }
            cubelist.Clear();
        }
        else
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
                    cubelist.Add(cube);
                }
                sy += CellSizeY;
            }
        }
    }


    LevelDesc GetLevelDescs()
    {
        LevelDesc ld = new LevelDesc();
        ld.Walls = new List<WallDesc>();
        ld.Obstacles = new List<ObstacleDesc>();
        ld.MapObjects = new List<MapDesc>();
        Transform enterTF = GameObject.Find("EnterPos").transform;
        ld.EnterPos = new Vector2(enterTF.position.x, enterTF.position.z);
        Transform exitTF = GameObject.Find("ExitPos").transform;
        ld.ExitPos = new Vector2(exitTF.position.x, exitTF.position.z);
        ld.NumCell = NumCell;
        ld.SpaceSize = SpaceSize;
        ld.StartPos = StartPos;

        ld.Heightmap = heightmap;
        ld.heightmapSampleRect = heightmapSampleRect;
        ld.heightmapUVOffset = heightmapUVOffset;

        GameObject wallRoot = GameObject.Find("WallRoot");
        int wallsCount = wallRoot.transform.childCount;
        for (int i = 0; i < wallsCount; ++i)
        {
            Transform wallTF = wallRoot.transform.GetChild(i);
            WallDesc wd = new WallDesc();
            wd.assetPath = GetResourcePath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(wallTF.gameObject)));
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
            od.assetPath = GetResourcePath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(obstacleTF.gameObject)));
            od.pos = obstacleTF.position;
            od.rot = obstacleTF.rotation;

            ld.Obstacles.Add(od);
        }

        GameObject mapRoot = GameObject.Find("MapRoot");
        int mapObjCount = mapRoot.transform.childCount;
        for (int j = 0; j < mapObjCount; ++j)
        {
            Transform mapObjTF = mapRoot.transform.GetChild(j);
            MapDesc md = new MapDesc();
            md.assetPath = GetResourcePath(AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(mapObjTF.gameObject)));
            md.pos = mapObjTF.position;
            md.rot = mapObjTF.rotation;

            ld.MapObjects.Add(md);
        }
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

    IEnumerator ExportHeightMap(string fileName)
    {
        yield return new WaitForEndOfFrame();
        string dataFilePath = Application.streamingAssetsPath + "/" + fileName;
        Texture2D tex_heightmap = new Texture2D(HeightmapConfig.heightmapTextureSizeX, HeightmapConfig.heightmapTextureSizeY, TextureFormat.RGB24, false);

        float sampleCountX = heightmapSampleRect.width / HeightmapConfig.heightmapSampleUnitSizeX;
        float sampleCountY = heightmapSampleRect.height / HeightmapConfig.heightmapSampleUnitSizeY;
        
        RaycastHit hit;
        Vector3 rayPos;
        Ray ray;
        Color color;
        for (int x = 0; x < sampleCountX; ++x)
        {
            for (int y = 0; y < sampleCountY; ++y)
            {
                color = Color.black;
                rayPos = new Vector3(x * HeightmapConfig.heightmapSampleUnitSizeX + heightmapSampleRect.min.x, 100, y * HeightmapConfig.heightmapSampleUnitSizeY + heightmapSampleRect.min.y);
                ray = new Ray(rayPos, Vector3.down);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Ground")
                    {
                        color.r = hit.point.y / HeightmapConfig.heightmapMaxHeightDistance;
                    }
                    tex_heightmap.SetPixel((int)heightmapUVOffset.x + x, (int)heightmapUVOffset.y + y, color);
                }
            }
        }

        tex_heightmap.Apply();
        byte[] bytes = tex_heightmap.EncodeToPNG();
        string rpcText = System.Convert.ToBase64String(bytes);
        Debug.Log(rpcText);
        File.WriteAllBytes(dataFilePath, bytes);
        UnityEngine.Object.Destroy(tex_heightmap);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 200, 40), "Display Cell Space Partition"))
        {
            DisplaysCellSpacePartition();
        }
        if (GUI.Button(new Rect(10, 70, 200, 40), "Export Level Desc Xml"))
        {
            ExportLevelDesc(fileName);
        }
        if (GUI.Button(new Rect(10, 130, 200, 40), "Export HeightMap"))
        {
            StartCoroutine(ExportHeightMap(heightmap));
        }
    }  
}
