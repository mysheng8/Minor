using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : BaseEntity
{
    public float m_StopDistance = 1.0f;
    public float m_BRadius = 1.0f;
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

    public float StopDistance
    {
        get
        {
            return m_StopDistance;
        }
        set
        {
            m_StopDistance = value;
        }
    }

	// Use this for initialization
    public void Awake()
    {
        base.Awake();
        IsStatic = true;
        m_Pos = new Vector2(transform.position.x, transform.position.z);

	}
}


