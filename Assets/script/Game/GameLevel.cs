using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameLevel
{
    LevelDesc m_ld;
    Vector3 m_LevelOffset;

    CellSpacePartition m_Partition;

    List<GameObject> m_mapObjects;
    List<Wall> m_Walls;
    List<Obstacle> m_Obstacles;

    Texture2D m_Heightmap;


    public List<Wall> Walls()
    {
        return m_Walls;
    }

    public List<Obstacle> Obstacles()
    {
        return m_Obstacles;
    }

    public CellSpacePartition Partition()
    {
        return m_Partition;
    }


    public Vector3 ExitPos()
    {
        return m_ld.ExitPos + m_LevelOffset;
    }

    public Vector3 EnterPos()
    {
        return m_ld.EnterPos + m_LevelOffset;
    }


    public Vector2 NumCell
    {
        get
        {
            return m_ld.NumCell;
        }    
    }

    public Vector2 SpaceSize
    {
        get
        {
            return m_ld.SpaceSize;
        }
    }

    public Vector2 StartPos
    {
        get
        {
            return m_ld.StartPos;
        }
    }

    // Use this for initialization
    public void Init(Vector3 offsePos, string fileName)
    {
        m_mapObjects = new List<GameObject>();
        m_Walls = new List<Wall>();
        m_Obstacles = new List<Obstacle>();
        m_Partition = new CellSpacePartition();
        m_LevelOffset = offsePos;
        LoadDescXML(fileName);
        LoadHeightmap(m_ld.Heightmap);
        m_Partition.Init(m_ld.SpaceSize.x, m_ld.SpaceSize.y, (int)m_ld.NumCell.x, (int)m_ld.NumCell.y, m_ld.StartPos.x, m_ld.StartPos.y, m_LevelOffset.x, m_LevelOffset.y);
        AddEntitys();
    }

    void LoadDescXML(string fileName)
    {
        string dataFilePath = Application.streamingAssetsPath + "/" + fileName;
        if (XmlUtils.hasFile(dataFilePath))
        {
            string dataString = XmlUtils.loadXML(dataFilePath);
            LevelDesc ld = XmlUtils.DeserializeObject(dataString, typeof(LevelDesc)) as LevelDesc;
            if (ld != null)
            { 
                m_ld = ld;
            }
        }
    }

    void LoadHeightmap(string fileName)
    {
        string dataFilePath = Application.streamingAssetsPath + "/" + fileName;
        FileStream fileStream = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        m_Heightmap = new Texture2D(HeightmapConfig.heightmapTextureSizeX, HeightmapConfig.heightmapTextureSizeX);
        m_Heightmap.LoadImage(bytes);   
    }

    void AddEntitys()
    {
        foreach (MapDesc m in m_ld.MapObjects)
        {
            Vector3 pos = m.pos + m_LevelOffset;
            Quaternion rot = m.rot;
            GameObject res = Resources.Load(m.assetPath) as GameObject;
            GameObject pref = UnityEngine.Object.Instantiate(res, pos, rot) as GameObject;
            m_mapObjects.Add(pref);
        }

        foreach (WallDesc w in m_ld.Walls)
        {
            Vector3 pos = w.pos + m_LevelOffset;
            Quaternion rot = w.rot;
            GameObject res = Resources.Load(w.assetPath) as GameObject;
            GameObject pref = UnityEngine.Object.Instantiate(res, pos, rot) as GameObject;
            //add wall to list
            m_Walls.Add(pref.GetComponent<Wall>());
        }

        foreach (ObstacleDesc o in m_ld.Obstacles)
        {
            Vector3 pos = o.pos + m_LevelOffset;
            Quaternion rot = o.rot;
            GameObject res = Resources.Load(o.assetPath) as GameObject;
            GameObject pref = UnityEngine.Object.Instantiate(res, pos, rot) as GameObject;
            ObstacleData data = o.data;
            Type t = ObstacleTable.GetObstacleType(data.ToString());
            Obstacle ent = pref.AddComponent(t) as Obstacle;
            ent.InitData(data, this);
            //Debug.Log(data.ToString());
            //Obstacle ent = pref.GetComponent<Obstacle>();
            //add obstacles to list
            m_Obstacles.Add(ent);

            //add to cell
            m_Partition.AddEntity((BaseEntity)ent); ;
        }
    }

    public float GetHeight(Vector2 pos)
    {
        int sampleX = (int)((pos.x - m_LevelOffset.x - m_ld.heightmapSampleRect.min.x) / HeightmapConfig.heightmapSampleUnitSizeX + m_ld.heightmapUVOffset.x);
        int sampleY = (int)((pos.y - m_LevelOffset.z - m_ld.heightmapSampleRect.min.y) / HeightmapConfig.heightmapSampleUnitSizeY + m_ld.heightmapUVOffset.y);

        Color color = m_Heightmap.GetPixel(sampleX, sampleY);

        return color.r*HeightmapConfig.heightmapMaxHeightDistance+ m_LevelOffset.y;
    }

    public void RemoveAll()
    {
        foreach (Wall w in m_Walls)
        {
            UnityEngine.Object.Destroy(w.gameObject, 0);
        }
        foreach (Obstacle o in m_Obstacles)
        {
            UnityEngine.Object.Destroy(o.gameObject, 0);
        }
        foreach (GameObject m in m_mapObjects)
        {
            UnityEngine.Object.Destroy(m, 0);
        }
        UnityEngine.Object.Destroy(m_Heightmap, 0);
    }

    public bool IsExceedCellSpace(Vector2 pos)
    {
        return (pos.x >= m_ld.SpaceSize.x + m_ld.StartPos.x + m_LevelOffset.x);
    }


    public bool IsBehindCellSpace(Vector2 pos, out Vector2 outPos)
    {
        outPos = pos;
        if (pos.x <= m_ld.StartPos.x + m_LevelOffset.x)
        {
            outPos.x = m_ld.StartPos.x + m_LevelOffset.x + 20;
            return true;
        }
        return false;
    }

    public bool IsExceedMap(Vector2 pos)
    {
        return (pos.x >= m_ld.heightmapSampleRect.xMax + m_LevelOffset.x);
    }

    public bool IsBehindMap(Vector2 pos)
    {
        return (pos.x <= m_ld.heightmapSampleRect.xMin + m_LevelOffset.x);
    }

    public void SetActive()
    {
        foreach (Obstacle o in m_Obstacles)
        {
            o.IsActive = true;
        }
        foreach (Wall w in m_Walls)
        {
            w.IsActive = true;
        }
    }

    public void SetDeActive()
    {
        foreach (Obstacle o in m_Obstacles)
        {
            o.IsActive = false;
        }
        foreach (Wall w in m_Walls)
        {
            w.IsActive = false;
        }
    }

}
