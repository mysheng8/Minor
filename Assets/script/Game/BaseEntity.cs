
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BaseEntity : MonoBehaviour {

    protected Vector2 m_Pos;
    protected bool m_Static = true;
    protected bool m_Damageable = false;
    protected bool m_Tagged = false;
    protected bool m_NonPenetrationConstraint = true;
    protected bool m_Jumpable = false;

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

	// Use this for initialization
    public void Awake()
    {
        m_Pos = Vector2.zero;
        m_World = GameWorld.Instance;

    }
	
	// Update is called once per frame
    public void Update(){ }

    
    public virtual bool HitTest(Vector2 entityPos, float entityRadius) { return false; }
    public virtual Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius) { return Vector2.zero; }

    public virtual bool HandleMessage(Telegram msg) { return true; }

}
