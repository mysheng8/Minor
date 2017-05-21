using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardMovement : MovementInterface
{
    Vector3 m_targetPos;
    Vector3 m_Position;
    Vector3 m_Dir = Vector3.zero;
    Quaternion m_Rotation;

    float m_Speed;
    bool m_Arrived;

    public ForwardMovement(Vector3 target, Vector3 source, float speed)
    {
        m_Speed = speed;
        m_Position = source;
        m_targetPos = target;
        m_Arrived = false;
    }
    
    public void UpdateTransform() 
    {
        m_Dir = m_targetPos - m_Position;
        float distance = m_Dir.sqrMagnitude;
        if (distance <= 0.01f)
        {
            m_Arrived = true;
            return;
        }
        Vector3 Velocity = m_Dir / distance * m_Speed;
        m_Position += Velocity * Time.deltaTime;
        m_Rotation = Quaternion.LookRotation(Velocity.normalized);
    }

    public bool IsArrived()
    { 
        return m_Arrived;
    }

    public Vector3 GetPosition()
    {
        return m_Position;
    }

    public Quaternion GetRotation()
    {
        return m_Rotation;
    }

    public Vector2 GetHeading()
    {
        return new Vector2(m_Dir.x, m_Dir.z);
    }
}

