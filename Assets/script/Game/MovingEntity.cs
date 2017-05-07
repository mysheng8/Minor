﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntity : BaseEntity
{
    protected Vector2 m_Heading = Vector2.zero;
    protected Vector2 m_Side = Vector2.zero;
    protected Vector2 m_LastPosInCellSpace;
    public float m_BRadius = 1.0f;
    Vector2 m_Velocity;
    float m_Mass = 0.1f;
    float m_MaxSpeed = 20;
    float m_MaxForce = 80;
    float m_MaxTurnRate = 2;
    float m_Damping = 0.05f;
    bool m_OnGround = true;
    
    BaseEntity m_Target;//move target 
    SteeringBehaviors m_Steering;
    Vector2 m_TeamOffset;

    public Vector2 LastPosInCellSpace
    {
        get
        {
            return m_LastPosInCellSpace;
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


    public BaseEntity Target
    {
        get
        {
            return m_Target;
        }
        set
        {
            m_Target = value;
        }
    }

    public Vector2 TeamOffset
    {
        get
        {
            return m_TeamOffset;
        }
        set
        {
            m_TeamOffset = value;
        }
    }

    public float MaxSpeed
    {
        get
        {
            return m_MaxSpeed;
        }
        set
        {
            m_MaxSpeed = value;
        }
    }

    public Vector2 Velocity
    {
        get
        {
             return m_Velocity;  
        }
        set
        {
            m_Velocity= value;
        }
        
    }

    public float Speed
    {
        get
        {
            return m_Velocity.magnitude;
        }
    }

    public float MaxForce
    {
        get
        {
            return m_MaxForce;
        }
    }

    public SteeringBehaviors Steering
    {
        get
        {
            return m_Steering;
        }
    }

    public bool OnGround
    {
        get
        {
            return m_OnGround;
        }
        set
        {
            m_OnGround = value;
        }
    }

	// Use this for initialization
    protected void Awake() 
    {
        base.Awake();
        m_Velocity = Vector2.right;
        m_Pos = new Vector2(transform.position.x, transform.position.z);
        m_Steering = new SteeringBehaviors(this);

	}

    float timer = 0;
	// Update is called once per frame
	protected void Update () 
    {
        if (m_World.isEditorMode)
            return;
        IsStatic = false;
        //update position in Cell
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            m_World.Partition.UpdateEntity(this, m_LastPosInCellSpace);
            m_LastPosInCellSpace = m_Pos;
            timer = Config.NumSecondUpdateEntityPosition;
        }

        m_World.Partition.CalculateNeighbors(m_Pos);
        
        Vector2 SteeringForce = m_Steering.Calculate();
        Vector2 acceleration = SteeringForce / m_Mass;
        m_Velocity += acceleration * Time.deltaTime ;
        if (m_Velocity.magnitude > 0.0000001)
        {
            m_Heading = m_Velocity.normalized;
            m_Side = new Vector2(m_Heading.y, -m_Heading.x);
            m_Velocity -= m_Heading * m_Damping ;
        }
        m_Velocity = Vector2.ClampMagnitude(m_Velocity, m_MaxSpeed);
        m_Pos += m_Velocity * Time.deltaTime;

        //Vector3 pos = transform.position;
        //Vector3 f1 = pos;
        //f1 += new Vector3(SteeringForce.x, 0, SteeringForce.y);
        //Vector3 f2 = pos;
        //f2 += new Vector3(m_Heading.x * 2, 0, m_Heading.y * 2);
        //Debug.DrawLine(pos, f1);
        //Debug.DrawLine(pos, f2); 

        //Vector3 line=gameObject.transform.position+new Vector3(acceleration.x,0,acceleration.y);
        //Debug.DrawLine(gameObject.transform.position, line);

	}

    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        return yMath.CircleHitTest(Pos, BRadius, entityPos, entityRadius);
    }

    public override Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        return yMath.CalculateCircleOverlay(Pos, BRadius, entityPos, entityRadius);
    }

}
