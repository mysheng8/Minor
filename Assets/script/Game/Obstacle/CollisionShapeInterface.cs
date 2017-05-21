using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface CollisionShapeInterface
{
    void UpdateCollision(Vector2 pos, float radius, Vector2 heading);
    bool HitTest(Vector2 entityPos, float entityRadius);
    Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius);
    void SetRadius(float radius);
}


public class CircleCollision : CollisionShapeInterface
{
    Vector2 m_Pos;
    float m_Radius;

    public CircleCollision(Vector2 pos, float radius)
    {
        m_Pos = pos;
        m_Radius = radius;
    }

    public void UpdateCollision(Vector2 pos, float radius, Vector2 heading)
    {
        m_Pos = pos;
        m_Radius = radius;
    }

    public bool HitTest(Vector2 entityPos, float entityRadius)
    {
        return yMath.CircleHitTest(m_Pos, m_Radius, entityPos, entityRadius);
    }

    public Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        Vector2 overlay = yMath.CalculateCircleOverlay(m_Pos, m_Radius, entityPos, entityRadius);
        //Debug.Log("CircleCollision" + overlay + m_Pos + m_Radius + m_Radius + entityRadius);
        return overlay;
    }

    public void SetRadius(float radius)
    {
        m_Radius = radius;
    }
}


public class RectCollision : CollisionShapeInterface
{
    Vector2 m_Pos;
    float m_Radius;
    Vector2 m_Heading = Vector2.zero;
    Vector2 m_Side = Vector2.zero;
    public Rect ObstacleRegion;

    public bool HitTest(Vector2 entityPos, float entityRadius)
    {
        Vector2 localPos = yMath.PointToLocalSpace(entityPos, m_Heading, m_Side, m_Pos);
        return yMath.RectHitTest(ObstacleRegion, m_Radius, localPos, entityRadius);
    }

    public Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        Vector2 localPos = yMath.PointToLocalSpace(entityPos, m_Heading, m_Side, m_Pos);
        Vector2 localMove = yMath.CalculateRectOverlay(ObstacleRegion, m_Radius, localPos, entityRadius);
        //return localMove;
        
        Vector2 overlay = yMath.VectorToWorldSpace(localMove, m_Heading, m_Side);
        //Debug.Log("RectCollision" + overlay + m_Pos + m_Heading + m_Radius + ObstacleRegion + entityPos + entityRadius);
        return overlay;
    }
    // Use this for initialization
    public RectCollision(Vector2 pos, float radius, Vector2 heading, Rect region)
    {
        ObstacleRegion = region;
        UpdateCollision(pos, radius, heading);
    }

    public void UpdateCollision(Vector2 pos, float radius, Vector2 heading)
    {
        m_Pos = pos;
        m_Radius = radius;
        m_Heading = heading;
        m_Side = new Vector2(m_Heading.y, -m_Heading.x);
    }

    public void SetRadius(float radius)
    {
        m_Radius = radius;
    }

}
