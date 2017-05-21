using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StaticCircleObstacle : Obstacle
{
    public ObstacleData m_Data;
    GameLevel m_Level;
    void Awake()
    {
        base.Awake();
        m_Collision = new CircleCollision(Pos, BRadius);
	}

    public override void InitData(ObstacleData data, GameLevel level)
    {
        m_Data = data;
        BRadius = data.BRadius;
        m_Level = level;
        m_Collision.SetRadius(BRadius);
    }

    public override ObstacleData Data
    {
        get
        {
            m_Data.BRadius = BRadius;
            return m_Data;
        }
    }
}
