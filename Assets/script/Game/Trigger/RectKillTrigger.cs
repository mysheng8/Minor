using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;


[Serializable]
public class RectKillIntervalTriggerData : TriggerData
{
    //[HideInInspector]
    public float xMin;
    //[HideInInspector]
    public float yMin;
    //[HideInInspector]
    public float width;
    //[HideInInspector]
    public float height;

    public int Lifetime;
    public int RemainingLifetime;
    public int NumUpdateBetweenRespawns;
    public int RemainingNumUpdatesUntilRespawn;
}






public class RectKillIntervalTrigger : Interval_Trigger
{
    //public Rect ObstacleRegion;
    public RectKillIntervalTriggerData m_Data;
    GameLevel m_Level;
    public Rect m_Region;

    void Start()
    {
        Vector2 heading = new Vector2(transform.forward.x, transform.forward.z).normalized;
        m_RegionOfInfluence = new RectCollision(Pos, 2, heading, m_Region);
        TriggerEvent += KillMinor;
    }
    public override void InitData(TriggerData data, GameLevel level)
    {
        m_Data = data as RectKillIntervalTriggerData;
        m_Region = new Rect(m_Data.xMin, m_Data.yMin, m_Data.width, m_Data.height);
        BRadius = m_Data.BRadius;
        m_Level = level;
        m_Lifetime = m_Data.Lifetime;
        m_RemainingLifetime = m_Data.RemainingLifetime;
        m_NumUpdateBetweenRespawns = m_Data.NumUpdateBetweenRespawns;
        m_RemainingNumUpdatesUntilRespawn = m_Data.RemainingNumUpdatesUntilRespawn;
    }
    public override TriggerData Data
    {
        get
        {
            m_Data.BRadius = BRadius;
            m_Data.xMin = m_Region.x;
            m_Data.yMin = m_Region.y;
            m_Data.width = m_Region.width;
            m_Data.height = m_Region.height;
            m_Data.Lifetime = m_Lifetime;
            m_Data.RemainingLifetime = m_RemainingLifetime;
            m_Data.NumUpdateBetweenRespawns = m_NumUpdateBetweenRespawns;
            m_Data.RemainingNumUpdatesUntilRespawn = m_RemainingNumUpdatesUntilRespawn;
            return m_Data;
        }
    }

    public void KillMinor(BaseEntity m)
    {
        m.RemoveFromGame = true;
    }
}
