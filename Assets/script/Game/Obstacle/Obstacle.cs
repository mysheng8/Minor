using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;


[Serializable]
[XmlInclude(typeof(FollowPathRectObstacleData))]
[XmlInclude(typeof(FollowPathCircleObstacleData))]
[XmlInclude(typeof(StaticRectObstacleData))]
public class ObstacleData
{
    [HideInInspector]
    public float BRadius;
}

public abstract class  Obstacle : BaseEntity
{
    protected CollisionShapeInterface m_Collision;

	// Use this for initialization
    public void Awake()
    {
        base.Awake();
        EType = EntityType.Obstacle;
        IsStatic = true;
        m_Pos = new Vector2(transform.position.x, transform.position.z);

	}

    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        return m_Collision.HitTest(entityPos, entityRadius);
    }

    public override Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        return m_Collision.CalculatePenetrationConstraint(entityPos, entityRadius);
    }

    public virtual void InitData(ObstacleData data, GameLevel level) { }
    public abstract ObstacleData Data { get; }



}

[Serializable]
public abstract class MovingObstacle : Obstacle
{
    //protected CollisionShapeInterface m_Collision;
    protected MovementInterface m_Movement;
    // Use this for initialization
    public void Awake()
    {
        base.Awake();
        IsStatic = false;
    }
    /*
    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        return m_Collision.HitTest(entityPos, entityRadius);
    }

    public override Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        return m_Collision.CalculatePenetrationConstraint(entityPos, entityRadius);
    }*/
}


public static class ObstacleTable
{
    private static Hashtable Table = InitHashtable();

    private static Hashtable InitHashtable()
    {
        Hashtable ht = new Hashtable();
        ht.Add("StaticRectObstacleData", typeof(StaticRectObstacle));
        ht.Add("ObstacleData", typeof(StaticCircleObstacle));
        ht.Add("FollowPathRectObstacleData", typeof(FollowPathRectObstacle));
        ht.Add("FollowPathCircleObstacleData", typeof(FollowPathCircleObstacle));
        return ht;
    }
    public static Type GetObstacleType(String text)
    {
        return (Type)Table[text];
    }
}
