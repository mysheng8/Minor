using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaMovement : MovementInterface
{
    Vector3 m_targetPos;
    Vector3 m_Position;
    Vector3 m_Dir = Vector3.zero;
    Quaternion m_Rotation;

    float m_Speed;
    bool m_Arrived;

    public ParabolaMovement(Vector3 target, Vector3 source, float speed)
    {
        m_Speed = speed;
        m_Position = source;
        m_targetPos = target;
        m_Arrived = false;
    }

    public void UpdateTransform()
    {
        Vector3 dir = m_targetPos - m_Position;
        float distance = dir.sqrMagnitude;
        if (distance <= 0.01f)
        {
            m_Arrived = true;
            return;
        }
        float t = distance / m_Speed;
        Vector3 Velocity = new Vector3(dir.x / t, dir.y / t + 0.5f * Config.Gravity * t, dir.z / t);
        Velocity.y -= Config.Gravity * Time.deltaTime;

        m_Position += Velocity * Time.deltaTime;
        m_Rotation = Quaternion.LookRotation(Velocity.normalized);
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

    public bool IsArrived()
    {
        return m_Arrived;
    }
}
