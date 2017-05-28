using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;


[Serializable]
public class TriggerData
{
    [HideInInspector]
    public float BRadius;
}


public abstract class Trigger : BaseEntity 
{
    CollisionShapeInterface m_RegionOfInfluence;

    bool m_RemoveFromGame;

    public void Awake()
    {
        base.Awake();
        EType = EntityType.Tigger;
        IsStatic = true;
        IsNonPenetrationConstraint = false;
        m_Pos = new Vector2(transform.position.x, transform.position.z);
    }

    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        if (m_Active)
            return m_RegionOfInfluence.HitTest(entityPos, entityRadius);
        else
            return false;
    }

    public virtual void InitData(TriggerData data, GameLevel level) { }
    public abstract TriggerData Data { get; }

    public bool RemoveFromGame
    {
        get
        { 
            return m_RemoveFromGame;
        }
        set
        {
            m_RemoveFromGame = value;
        }
    }

    public delegate void TryTrigger(Character entity);
    public event TryTrigger trytrigger;
}




public class Respawning_Trigger: Trigger
{
    int m_NumUpdateBetweenRespawns;
    int m_RemainingNumUpdatesRespawn;

    void Deactive()
    {
        IsActive=false;
        m_RemainingNumUpdatesRespawn = m_NumUpdateBetweenRespawns;
    }

    void Update()
    {
        if ((--m_RemainingNumUpdatesRespawn) <= 0 && !IsActive)
        {
            IsActive = true;
        }
    }
}

public class LimitedLifetime_Trigger : Trigger
{
    int m_Lifetime;
    void Update()
    {
        if (--m_Lifetime <= 0)
        {
            RemoveFromGame=true;
        }
    }
}

public class Interval_Trigger : Trigger
{
    int m_Lifetime;
    int m_RemainingLifetime;
    int m_NumUpdateBetweenRespawns;
    int m_RemainingNumUpdatesUntilRespawn;
    void Update()
    {
        if (IsActive)
        {
            if (--m_Lifetime <= 0)
            {
                IsActive = false;
                m_RemainingNumUpdatesUntilRespawn = m_NumUpdateBetweenRespawns;
            }
        }
        else
        {
            if ((--m_RemainingNumUpdatesUntilRespawn) <= 0)
            {
                IsActive = true;
            }
        }
    }
}
