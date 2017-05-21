using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[Serializable]
public class StaticRectObstacleData : ObstacleData
{
    [HideInInspector]
    public float xMin;
    [HideInInspector]
    public float yMin;
    [HideInInspector]
    public float width;
    [HideInInspector]
    public float height;
}


public class StaticRectObstacle : Obstacle
{
    //public Rect ObstacleRegion;
    public StaticRectObstacleData m_Data;
    GameLevel m_Level;
    public Rect m_Region;

    void Start()
    {
        Vector2 heading = new Vector2(transform.forward.x, transform.forward.z).normalized;
        m_Collision = new RectCollision(Pos, 2, heading, m_Region);
    }
    public override void InitData(ObstacleData data, GameLevel level)
    {
        m_Data = data as StaticRectObstacleData;
        m_Region = new Rect(m_Data.xMin, m_Data.yMin, m_Data.width, m_Data.height);
        BRadius = m_Data.BRadius;
        m_Level = level;
    }
    public override ObstacleData Data
    {
        get
        {
            m_Data.BRadius = BRadius;
            m_Data.xMin = m_Region.x;
            m_Data.yMin = m_Region.y;
            m_Data.width = m_Region.width;
            m_Data.height = m_Region.height;
            return m_Data;
        }
    }
}
