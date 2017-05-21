using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationMovement: MovementInterface
{
    float m_SpeedY = 0;
    Vector3 m_Position;
    Vector3 m_Dir;
    Quaternion m_Rotation;

    public RotationMovement(float speedy, Vector3 position, Vector3 dir)
    {
        m_SpeedY = speedy;
        m_Dir = dir;
        m_Position = position;
        m_Rotation = Quaternion.LookRotation(m_Dir);
    }

    public void UpdateTransform()
    {
        m_Dir = Vector3.RotateTowards(m_Dir, Vector3.up, m_SpeedY * Time.deltaTime, 0.0F);
        m_Rotation = Quaternion.LookRotation(m_Dir);
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