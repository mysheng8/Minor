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

    void Awake()
    {
        base.Awake();
        EType = EntityType.Wall;
        m_Normal = new Vector2(gameObject.transform.forward.x, gameObject.transform.forward.z);
        Vector2 Side = new Vector2(gameObject.transform.right.x,gameObject.transform.right.z);
        Pos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.z);
        m_From = yMath.PointToWorldSpace(fromOffset, m_Normal, Side, Pos);
        m_To = yMath.PointToWorldSpace(toOffset, m_Normal, Side, Pos);
    }

    void Update()
    {
        Vector3 dir = new Vector3(m_Normal.x, 0, m_Normal.y);
        Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + dir*10);
        Vector3 from = new Vector3(m_From.x, 50, m_From.y);
        Vector3 to = new Vector3(m_To.x, 50, m_To.y);
        Debug.DrawLine(from, to);
    }



}
