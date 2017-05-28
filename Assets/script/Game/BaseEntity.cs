using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType
{ 
    None,
    Character,
    Obstacle,
    Wall,
    Tigger
}

public class BaseEntity : MonoBehaviour {

    public int ID;
    protected Vector2 m_Pos = Vector2.zero;
    Vector2 m_LastPosInCellSpace;
    protected float m_Height = 0;
    protected bool m_Static = true;
    protected bool m_Damageable = false;
    protected bool m_Tagged = false;
    protected bool m_NonPenetrationConstraint = true;
    protected bool m_Jumpable = false;
    public float m_BRadius = 1.0f;
    protected GameWorld m_World;
    protected EntityType m_EType = EntityType.None;
    protected bool m_Active = false;

    public bool IsTagged
    { 
        get
        {
            return m_Tagged;
        }
        set
        {
            m_Tagged = value;
        }
    }

    public EntityType EType
    {
        get
        {
            return m_EType;
        }
        set
        {
            m_EType = value;
        }
    }

    public bool IsStatic
    {
        get 
        {
            return m_Static;
        }
        set 
        {
            m_Static = value;
        }
    }

    public bool IsDamageable
    {
        get
        {
            return m_Damageable;
        }
        set
        {
            m_Damageable = value;
        }
    }

    public bool IsJumpable
    {
        get
        {
            return m_Jumpable;
        }
        set
        {
            m_Jumpable = value;
        }
    }

    public bool IsNonPenetrationConstraint
    {
        get
        {
            return m_NonPenetrationConstraint;
        }
        set
        {
            m_NonPenetrationConstraint = value;
        }
    }

    public bool IsActive
    {
        get
        {
            return m_Active;
        }
        set
        {
            m_Active = value;
        }
    }

    public GameWorld World
    {
        get
        {
            return m_World;
        }
    }

    public float BRadius
    {
        get
        {
            return m_BRadius;
        }
        set
        {
            m_BRadius = value;
        }
    }

    public Vector2 Pos
    {
        get
        {
            return m_Pos;
        }
        set
        {
            m_Pos = value;
        }
    }

    public float Height
    {
        get
        {
            return m_Height;
        }
        set
        {
            m_Height = value;
        }
    }

    public Vector2 LastPosInCellSpace
    {
        get
        {
            return m_LastPosInCellSpace;
        }
    }

	// Use this for initialization
    public void Awake()
    {
        ID = GetInstanceID();
        m_World = GameWorld.Instance;
    }
    float timer = 0;
	// Update is called once per frame
    public void Update()
    {
        if (m_World.isEditorMode)
            return;
        if(!IsStatic && IsActive)
        {
            //update position in Cell
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                m_World.Partition.UpdateEntity(this, m_LastPosInCellSpace);
                m_LastPosInCellSpace = m_Pos;
                timer = Config.NumSecondUpdateEntityPosition;
            }

            m_World.Partition.CalculateNeighbors(m_Pos);   
        }
    }

    
    public virtual bool HitTest(Vector2 entityPos, float entityRadius) { return false; }
    public virtual Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius) { Debug.Log("base entity"); return Vector2.zero; }
    public virtual bool HandleMessage(Telegram msg) { return true; }

}
