using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathMovement : MovementInterface
{
    bool m_Return;
    int m_Next;
    int m_CurrentIndex;
    List<Vector3> m_PointList;
    Vector3 m_Position;
    Quaternion m_Rotation;
    Vector3 m_Dir = Vector3.zero;
    Vector3 m_InitPos;

    float m_Speed = 1;
    public FollowPathMovement(Vector3 initPos, float speed, List<Vector3> pointList, bool isReturn)
    {
        m_PointList = pointList;
        m_CurrentIndex = 0;
        m_Speed = speed;
        m_Next = 1;
        m_Return = isReturn;
        m_Position = m_PointList[m_CurrentIndex]+initPos;
        m_InitPos = initPos;
    }

    public void UpdateTransform()
    {

        if ( m_PointList.Count < 2 )
            return;
        if (m_CurrentIndex == m_PointList.Count - 1&&!m_Return)
            return;

        m_Dir = m_PointList[m_CurrentIndex + m_Next] - m_PointList[m_CurrentIndex];
        m_Position += m_Dir * m_Speed * Time.deltaTime;
        m_Rotation = Quaternion.LookRotation(m_Dir);


        float forward = Vector3.Dot((m_PointList[m_CurrentIndex + m_Next] + m_InitPos - m_Position), m_Dir);

        if (forward < 0)
        {
            m_CurrentIndex += m_Next;
        }
        if (m_Return)
        {
            if (m_CurrentIndex == m_PointList.Count - 1)
            {
                m_Next = -1;
            }
            if (m_CurrentIndex == 0)
            {
                m_Next = 1;
            }
        }
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
        return new Vector2(m_Dir.x, m_Dir.z).normalized;
    }
}
