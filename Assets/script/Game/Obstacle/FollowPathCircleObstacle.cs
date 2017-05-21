using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class FollowPathCircleObstacleData : ObstacleData
{
    public bool IsReturn;
    public float Speed;
    public List<Vector3> PointList = new List<Vector3>();
}


public class FollowPathCircleObstacle : MovingObstacle
{
    //public bool IsReturn;
    //public float Speed;
    //public List<Vector3> PointList = new List<Vector3>();
    public FollowPathCircleObstacleData m_Data = new FollowPathCircleObstacleData();
    GameLevel m_Level;
/*GameLevel m_Level;
    // Use this for initialization
    public void Awake()
    {
        base.Awake();
        IsStatic = false;

    }
*/
    public void Start()
    {
        if (World.isEditorMode)
            m_Movement = new FollowPathMovement(Vector3.zero, m_Data.Speed, m_Data.PointList, m_Data.IsReturn);
        else
            m_Movement = new FollowPathMovement(m_Level.EnterPos(), m_Data.Speed, m_Data.PointList, m_Data.IsReturn);
        m_Collision = new CircleCollision(Pos, BRadius);    
    }

    public override void InitData(ObstacleData data, GameLevel level)
    {
        m_Data = data as FollowPathCircleObstacleData;
        BRadius = m_Data.BRadius;
        m_Level = level;
    }
    public override ObstacleData Data
    {
        get
        {
            m_Data.BRadius = BRadius;
            return m_Data;
        }
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        m_Movement.UpdateTransform();
        Vector3 pos = m_Movement.GetPosition();
        m_Pos = new Vector2(pos.x, pos.z);

        m_Collision.UpdateCollision(Pos, BRadius, m_Movement.GetHeading());
        gameObject.transform.position = m_Movement.GetPosition();
        gameObject.transform.rotation = m_Movement.GetRotation();
        
    }
}


