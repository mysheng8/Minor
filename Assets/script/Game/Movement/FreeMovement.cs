using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MovementInterface
{
    void UpdateTransform();
    Vector3 GetPosition();
    Quaternion GetRotation();
    Vector2 GetHeading();
}


public class FreeMovement : MovementInterface
{
    Vector2 m_Heading = Vector2.zero;
    Vector2 m_Side = Vector2.zero;
    Character m_Entity;
    //Vector2 m_Pos;
    Vector2 m_Velocity;
    //float m_Height = 0;
    float m_Mass = 0.1f;
    float m_MaxSpeed = 20;
    float m_MaxForce = 100;
    float m_MaxTurnRate = 2;
    float m_Damping = 0.05f;
    
    
    Character m_Target;//move target 
    SteeringBehaviors m_Steering;
    Vector2 m_TeamOffset;

    public Vector2 Pos
    {
        get
        {
            return m_Entity.Pos;
        }
        set
        {
            m_Entity.Pos = value;
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


    public Character Target
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

    public float Height
    {
        get
        {
            return m_Entity.Height;
        }
        set
        {
            m_Entity.Height = value;
        }
    }

    public SteeringBehaviors Steering
    {
        get
        {
            return m_Steering;
        }
    }



	// Use this for initialization
    public FreeMovement(Character entity) 
    {
        m_Velocity = Vector2.zero;
        //m_Pos = entity.Pos;
        m_Entity = entity;
        m_Steering = new SteeringBehaviors(entity);
	}


	// Update is called once per frame
    public void UpdateTransform()
    {
        //m_Pos = m_Entity.Pos;
        Vector2 SteeringForce = m_Steering.Calculate();
        Vector2 acceleration = SteeringForce / m_Mass;
        m_Velocity += acceleration * Time.deltaTime ;
        if (m_Velocity.magnitude > 0.0000001f)
        {
            m_Heading = m_Velocity.normalized;
            m_Side = new Vector2(m_Heading.y, -m_Heading.x);
            m_Velocity -= m_Heading * m_Damping ;
        }
        m_Velocity = Vector2.ClampMagnitude(m_Velocity, m_MaxSpeed);
        Pos += m_Velocity * Time.deltaTime;
        /*
        Vector3 pos = GetPosition();
        Vector3 posMove0 = new Vector3(m_Steering.Target.x, Height, m_Steering.Target.y);
        Vector3 posMove1 = pos + new Vector3(SteeringForce.x, Height, SteeringForce.y);
        Debug.DrawLine(pos, posMove1);
        //Debug.DrawLine(pos, posMove1);*/
	}

    public Vector3 GetPosition()
    {
        return new Vector3(Pos.x,Height,Pos.y);
    }

    public Quaternion GetRotation()
    {
        return Quaternion.LookRotation(new Vector3(m_Heading.x, 0, m_Heading.y));
    }

    public Vector2 GetHeading()
    {
        return m_Heading;
    }

}
