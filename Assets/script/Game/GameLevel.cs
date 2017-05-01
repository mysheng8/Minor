using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel
{

    LevelDesc m_ld;
    Vector2 m_LevelOffset;

    CellSpacePartition m_Partition;

    List<Wall> m_Walls;
    List<StaticEntity> m_Obstacles;

    GameObject mapObject;

    public List<Wall> Walls()
    {
        return m_Walls;
    }

    public List<StaticEntity> Obstacles()
    {
        return m_Obstacles;
    }

    public CellSpacePartition Partition()
    {
        return m_Partition;
    }


    public Vector2 ExitPos()
    {
        return m_ld.ExitPos + m_LevelOffset;
    }


	// Use this for initialization
    public void Init(Vector2 offsePos, string fileName)
    {
        m_Walls = new List<Wall>();
        m_Obstacles = new List<StaticEntity>();
        m_Partition = new CellSpacePartition(); 
        m_LevelOffset = offsePos;
        LoadDescXML(fileName);
        m_Partition.Init(m_ld.SpaceSize.x, m_ld.SpaceSize.y, (int)m_ld.NumCell.x, (int)m_ld.NumCell.y, m_ld.StartPos.x, m_ld.StartPos.y, m_LevelOffset.x, m_LevelOffset.y);
        AddWallsAndObstacles();

        Vector3 mapPos = m_ld.pos + new Vector3(m_LevelOffset.x, 0, m_LevelOffset.y);

        mapObject = Object.Instantiate(Resources.Load(m_ld.assetPath), mapPos, m_ld.rot) as GameObject;
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

    void AddWallsAndObstacles()
    {
        foreach (WallDesc w in m_ld.Walls)
        {
            Vector3 pos = w.pos + new Vector3(m_LevelOffset.x, 0, m_LevelOffset.y);
            Quaternion rot = w.rot;
            GameObject res = Resources.Load(w.assetPath) as GameObject;
            GameObject pref = Object.Instantiate(res, pos, rot) as GameObject;
            m_Walls.Add(pref.GetComponent<Wall>());
        }

        foreach (ObstacleDesc o in m_ld.Obstacles)
        {
            Vector3 pos = o.pos + new Vector3(m_LevelOffset.x, 0, m_LevelOffset.y);
            Quaternion rot = o.rot;
            GameObject res = Resources.Load(o.assetPath) as GameObject;
            GameObject pref = Object.Instantiate(res, pos, rot) as GameObject;
            StaticEntity ent = pref.GetComponent<StaticEntity>();

            m_Obstacles.Add(ent);

            //add to cell
            m_Partition.AddEntity((BaseEntity)ent); ;
        }
    }

    public void RemoveAll()
    {
        foreach (Wall w in m_Walls)
        {
            Object.Destroy(w.gameObject, 0);
        }
        foreach (StaticEntity o in m_Obstacles)
        {
            Object.Destroy(o.gameObject, 0);
        }
        Object.Destroy(mapObject, 0);
    }

    public bool OverMap(Vector2 pos)
    {
        return (pos.x >= m_ld.SpaceSize.x + m_ld.StartPos.x + m_LevelOffset.x);
    }

    public bool OutOfMap(Vector2 pos, out Vector2 outPos)
    {
        outPos = pos;
        if (pos.x <= m_ld.StartPos.x + m_LevelOffset.x)
        {
            outPos.x = m_ld.StartPos.x + m_LevelOffset.x + 20; ;
            return true;
        }
        return false;
    }


}
