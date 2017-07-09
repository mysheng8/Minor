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
    public delegate void TriggerHandler(BaseEntity Receiver);
    public event TriggerHandler TriggerEvent;
    public CollisionShapeInterface m_RegionOfInfluence;

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

    public void TryTrigger(Character m)
    {
        if (HitTest(m.Pos, m.BRadius))
        {
            TriggerEvent.Invoke(m);
        }
    }

}


public abstract class Respawning_Trigger: Trigger
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
        base.Update();
        if ((--m_RemainingNumUpdatesRespawn) <= 0 && !IsActive)
        {
            IsActive = true;
        }
    }
}

public abstract class LimitedLifetime_Trigger : Trigger
{
    int m_Lifetime;
    void Update()
    {
        base.Update();
        if (--m_Lifetime <= 0)
        {
            RemoveFromGame=true;
        }
    }
}

public abstract class Interval_Trigger : Trigger
{
    protected int m_Lifetime;
    protected int m_RemainingLifetime;
    protected int m_NumUpdateBetweenRespawns;
    protected int m_RemainingNumUpdatesUntilRespawn;
    void Update()
    {
        base.Update();
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
