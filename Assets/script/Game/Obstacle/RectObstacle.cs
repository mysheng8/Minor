using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectObstacle : Obstacle
{
    Vector2 m_Heading = Vector2.zero;
    Vector2 m_Side = Vector2.zero;
    public Rect ObstacleRegion;
    



    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        Vector2 localPos = yMath.PointToLocalSpace(entityPos, m_Heading, m_Side, Pos);
        return yMath.RectHitTest(ObstacleRegion, StopDistance, localPos, entityRadius);
    }

    public override Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        Vector2 localPos = yMath.PointToLocalSpace(entityPos, m_Heading, m_Side, Pos);
        Vector2 localMove = yMath.CalculateRectOverlay(ObstacleRegion, StopDistance, localPos, entityRadius);
        //return localMove;

        return yMath.VectorToWorldSpace(localMove, m_Heading, m_Side);
    }
    // Use this for initialization
    public void Awake()
    {
        base.Awake();
        Vector3 zAxis = gameObject.transform.forward;
        m_Heading = new Vector2(zAxis.x, zAxis.z);
        m_Heading = m_Heading.normalized;
        m_Side = new Vector2(m_Heading.y, -m_Heading.x);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(Pos.x, 0, Pos.y);
        cube.transform.rotation = Quaternion.LookRotation(new Vector3(m_Side.x, 0, m_Side.y));
        cube.transform.localScale = new Vector3(ObstacleRegion.width, 100, ObstacleRegion.height);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
