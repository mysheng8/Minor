//#define EDITORMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BaseEntity : MonoBehaviour {

    protected Vector2 m_Pos;
    protected Vector2 m_Heading = Vector2.zero;
    protected Vector2 m_Side = Vector2.zero;
    protected Vector2 m_LastPos;
    float timer= 0;

    public float m_BRadius = 1.0f;
    bool m_UpdatePosition = false;
    protected bool m_Tagged = false;
    protected bool m_NonPenetrationConstraint = true;
    protected GameWorld m_World;

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

    public bool IsUpdatePosition
    {
        get 
        {
            return m_UpdatePosition;
        }
        set 
        {
            m_UpdatePosition = value;
        }
    }

    public GameWorld World
    {
        get
        {
            return m_World;
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
    public Vector2 LastPos
    {
        get
        {
            return m_LastPos;
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

    public Vector2 Heading
    {
        get
        {
            return m_Heading;
        }
    }

    public Vector2 Side
    {
        get
        {
            return m_Side;
        }
    }

	// Use this for initialization
    public void Awake()
    {
        m_Pos = Vector2.zero;
        m_World = GameWorld.Instance;

    }
	
	// Update is called once per frame
    public void Update()
    {
        //if (Application.isEditor)
        //    return;

        //update position in Cell
#if !EDITORMODE
        if (m_UpdatePosition)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                m_World.Partition.UpdateEntity(this, m_LastPos);
                m_LastPos = m_Pos;
                timer = Config.NumSecondUpdateEntityPosition;
            }
        }

        m_World.Partition.CalculateNeighbors(m_Pos);
#endif
    }

    public virtual bool HandleMessage(Telegram msg) { return true; }


    public virtual bool IsMovingEntity() { return false; }
    public virtual bool IsStaticEntity() { return false; }
    public virtual bool IsCharacter() { return false; }

}
