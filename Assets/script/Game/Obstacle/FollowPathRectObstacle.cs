using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
[Serializable]
public class SeVector3
{
    public float _x;
    public float _y;
    public float _z;
    public SeVector3()
    {
        _x = 0;
        _y = 0;
        _z = 0;
    }
    public SeVector3(float x, float y, float z)
    {
        _x = x;
        _y = y;
        _z = z;
    }

    public SeVector3(Vector3 v)
    {
        _x = v.x;
        _y = v.y;
        _z = v.z;
    }
    public Vector3 GetVector3()
    {
        return new Vector3(_x, _y, _z);
    }
}*/



[Serializable]
public class FollowPathRectObstacleData : ObstacleData
{
    public bool IsReturn;
    public float Speed;
    public List<Vector3> PointList = new List<Vector3>();
    [HideInInspector]
    public float xMin;
    [HideInInspector]
    public float yMin;
    [HideInInspector]
    public float width;
    [HideInInspector]
    public float height;
}





public class FollowPathRectObstacle : MovingObstacle
{
    /*
    public Rect ObstacleRegion;
    public bool IsReturn = true;
    public float Speed;
    public List<Vector3> m_PointList = new List<Vector3>();*/

    public FollowPathRectObstacleData m_Data = new FollowPathRectObstacleData();
    GameLevel m_Level;
    
    public Rect m_Region;


    private Vector2 m_Heading;
    public Vector2 Heading
    {
        get { return m_Heading; }
    }

    /*
    // Use this for initialization
    public void Awake()
    {
        base.Awake();
        IsStatic = false;
    }
    */
    public void Start()
    {
        if(World.isEditorMode)
            m_Movement = new FollowPathMovement(Vector3.zero, m_Data.Speed, m_Data.PointList, m_Data.IsReturn);
        else
            m_Movement = new FollowPathMovement(m_Level.EnterPos(), m_Data.Speed, m_Data.PointList, m_Data.IsReturn);
        m_Heading = new Vector2(transform.forward.x, transform.forward.z).normalized;
        m_Collision = new RectCollision(Pos, 2, m_Heading, m_Region);
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        m_Movement.UpdateTransform();
        Vector3 pos = m_Movement.GetPosition();
        m_Pos = new Vector2(pos.x, pos.z);
        m_Heading = m_Movement.GetHeading();
        m_Collision.UpdateCollision(Pos, 2, m_Heading);
        gameObject.transform.position = m_Movement.GetPosition();
        gameObject.transform.rotation = m_Movement.GetRotation();
        
    }

    public override void InitData(ObstacleData data, GameLevel level)
    {
        m_Data = data as FollowPathRectObstacleData;
        m_Region = new Rect(m_Data.xMin, m_Data.yMin, m_Data.width, m_Data.height);
        BRadius = m_Data.BRadius;
        m_Level = level;
    }

    public void UpdateData()
    {
        m_Data.BRadius = BRadius;
        m_Data.xMin = m_Region.x;
        m_Data.yMin = m_Region.y;
        m_Data.width = m_Region.width;
        m_Data.height = m_Region.height;
    }


    public override ObstacleData Data
    {
        get
        {
            UpdateData();
            return m_Data;
        }
    }


}
