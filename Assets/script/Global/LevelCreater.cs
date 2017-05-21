
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
    public ObstacleData data;
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
    public Vector3 EnterPos;
    public Vector3 ExitPos;

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

    LevelDesc GetLevelDescs()
    {
        LevelDesc ld = new LevelDesc();
        ld.Walls = new List<WallDesc>();
        ld.Obstacles = new List<ObstacleDesc>();
        ld.MapObjects = new List<MapDesc>();
        
        Transform enterTF = GameObject.Find("EnterPos").transform;
        ld.EnterPos = enterTF.position;
        Transform exitTF = GameObject.Find("ExitPos").transform;
        ld.ExitPos = exitTF.position;
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

           
            od.data = obstacleTF.GetComponent<Obstacle>().Data;
            Debug.Log(od.data);
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

    public void ExportLevelDesc(string fileName)
    {
        LevelDesc ld = GetLevelDescs();
        string dataFilePath = Application.streamingAssetsPath + "/" + fileName;
        string dataString = XmlUtils.SerializeObject(ld, typeof(LevelDesc));
        XmlUtils.CreateXML(dataFilePath, dataString);
    }

    public void ExportHeightMap(string fileName)
    {
        //yield return new WaitForEndOfFrame();
        GameObject obstacleRoot = GameObject.Find("ObstacleRoot");
        obstacleRoot.SetActive(false);
        GameObject wallRoot = GameObject.Find("WallRoot");
        wallRoot.SetActive(false);
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
        obstacleRoot.SetActive(true);
        wallRoot.SetActive(true);
        UnityEngine.Object.DestroyImmediate(tex_heightmap);
    }

}
