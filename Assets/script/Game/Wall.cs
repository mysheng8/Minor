using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : BaseEntity 
{
    public Vector2 fromOffset;
    public Vector2 toOffset;

    Vector2 m_From;
    Vector2 m_To;
    Vector2 m_Normal;
    Vector2 m_Pos;


    public Vector2 From()
    {
        return m_From;
    }

    public Vector2 To()
    {
        return m_To;
    }

    public Vector2 Normal()
    {
        return m_Normal;
    }

    public Vector2 Pos()
    {
        return m_Pos;
    }

    void Awake()
    {
        base.Awake();
        m_Normal = new Vector2(gameObject.transform.forward.x, gameObject.transform.forward.z);
        Vector2 Side = new Vector2(gameObject.transform.right.x,gameObject.transform.right.z);
        m_Pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        m_From = yMath.PointToWorldSpace(fromOffset, m_Normal, Side, m_Pos);
        m_To = yMath.PointToWorldSpace(toOffset, m_Normal, Side, m_Pos);
    }

    void Update()
    {
        Vector3 dir = new Vector3(m_Normal.x, 0, m_Normal.y);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + dir*10);
    }



}
