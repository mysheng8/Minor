using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SteeringBehaviors
{
    
    Character m_Entity;

    Vector2 m_SteeringForce;

    Vector2 m_Target;



    //Flee
    float m_PanicDistance = 10;

    //Wander
    float m_WanderRadius = 4;
    float m_WanderDistance = 4;
    float m_WanderJitter = 2;
    Vector2 m_vWanderTarget = Vector2.zero;

    //Wall Avoidance
    //feelers
    Vector2 feeleroffset0 = new Vector2(4, 2);
    Vector2 feeleroffset1 = new Vector2(4, -2);
    List<Vector2> feelers;
    float m_WallMaxDist = 999999;

    //Obstacle Avoidance
    float m_BrakingWeight = 0.1f;
    float m_ObstacleMaxDist = 10;

    bool m_Separation = false;
    bool m_Alignment = false;
    bool m_Cohesion = false;
    bool m_Crowd = false;
    bool m_WallAvoidance = false;
    bool m_ObstacleAvoidance = false;
    bool m_Wander = false;
    bool m_Seek = false;
    bool m_Arrive = false;
    bool m_Pursuit = false;
    bool m_OffsetPursuit = false;
    bool m_Evade = false;

    float m_dMultWallAvoidance = 30.0f;
    float m_dMultObstacleAvoidance = 30.0f;
    float m_dMultSeparation = 10.0f;
    float m_dMultCrowd = 8.0f;
    float m_dMultAlignment = 1.0f;
    float m_dMultCohesion = 0.5f;
    float m_dMultWander = 5.0f;
    float m_dMultSeek = 5.0f;
    float m_dMultArrive = 5.0f;
    float m_dMultPursuit = 5.0f;
    float m_dMultOffsetPursuit = 5.0f;
    float m_dMultEvade = 5.0f;

    float m_ViewDistance = 5;


    public bool IsSeparation
    {
        get
        {
            return m_Separation;
        }
        set
        {
            m_Separation = value;
        }
    }

    public bool IsAlignment
    {
        get
        {
            return m_Alignment;
        }
        set
        {
            m_Alignment = value;
        }
    }

    public bool IsCrowd
    {
        get
        {
            return m_Crowd;
        }
        set
        {
            m_Crowd = value;
        }
    }

    public bool IsCohesion
    {
        get
        {
            return m_Cohesion;
        }
        set
        {
            m_Cohesion = value;
        }
    }

    public bool IsWallAvoidance
    {
        get
        {
            return m_WallAvoidance;
        }
        set
        {
            m_WallAvoidance = value;
        }
    }

    public bool IsObstacleAvoidance
    {
        get
        {
            return m_ObstacleAvoidance;
        }
        set
        {
            m_ObstacleAvoidance = value;
        }
    }

    public bool IsWander
    {
        get
        {
            return m_Wander;
        }
        set
        {
            m_Wander = value;
        }
    }

    public bool IsSeek
    {
        get
        {
            return m_Seek;
        }
        set
        {
            m_Seek = value;
        }
    }

    public bool IsArrive
    {
        get
        {
            return m_Arrive;
        }
        set
        {
            m_Arrive = value;
        }
    }


    public bool IsPursuit
    {
        get
        {
            return m_Pursuit;
        }
        set
        {
            m_Pursuit = value;
        }
    }

    public bool IsOffsetPursuit
    {
        get
        {
            return m_OffsetPursuit;
        }
        set
        {
            m_OffsetPursuit = value;
        }
    }

    public bool IsEvade
    {
        get
        {
            return m_Evade;
        }
        set
        {
            m_Evade = value;
        }
    }

    public Vector2 Target
    {
        get
        {
            return m_Target;
        }
        set
        {
            m_Target = value;
        }
    }

    public SteeringBehaviors(Character entity)
    {
        m_Entity = entity;
        m_SteeringForce = Vector2.zero;
        feelers = new List<Vector2>();
    }


    public bool HasArrived()
    {
        float dis = (m_Target - m_Entity.Movement.Pos).sqrMagnitude;
        if (dis < 0.1f)
            return true;
        return false;
    }

 	Vector2 Seek (Vector2 TargetPos) 
    {
        Vector2 DesiredVelocity = (TargetPos - m_Entity.Movement.Pos).normalized * m_Entity.Movement.MaxSpeed;
        return (DesiredVelocity - m_Entity.Movement.Velocity);
	}

    Vector2 Flee(Vector2 TargetPos)
    {
        if (Vector2.Distance(TargetPos, m_Entity.Movement.Pos) > m_PanicDistance)
        {
            return Vector2.zero;
        }
        Vector2 DesiredVelocity = (m_Entity.Movement.Pos - TargetPos).normalized * m_Entity.Movement.MaxSpeed;
        return (DesiredVelocity - m_Entity.Movement.Velocity); 
    }

    Vector2 Arrive(Vector2 TargetPos)
    {
        Vector2 ToTarget = TargetPos - m_Entity.Movement.Pos;
        float dist = ToTarget.magnitude;
        
        if (dist > 0)
        {
            float DecelerationTweaker = 0.5f;
            float speed = dist / DecelerationTweaker;
            speed = Mathf.Min(speed, m_Entity.Movement.MaxSpeed);
            Vector2 DesiredVelocity = ToTarget * speed / dist;
            return (DesiredVelocity - m_Entity.Movement.Velocity);
        }
        return Vector2.zero;
    }

    Vector2 Pursuit(Character target)
    {
        Vector2 ToEvader = target.Pos - m_Entity.Movement.Pos;
        float relativeHeading = Vector2.Dot(target.Movement.Heading, m_Entity.Movement.Heading);
        if ((Vector2.Dot(ToEvader, m_Entity.Movement.Heading) > 0) && (relativeHeading < -0.95))//acos(0.95)=18deg
        {
            return Seek(target.Pos);
        }
        float LookAheadTime = ToEvader.magnitude / (m_Entity.Movement.MaxSpeed + target.Movement.Speed);
        return Seek(target.Pos + target.Movement.Velocity * LookAheadTime);
    }

    Vector2 Evade(Character target)
    {
        Vector2 ToPursuer = target.Pos - m_Entity.Movement.Pos;
        float LookAheadTime = ToPursuer.magnitude / (m_Entity.Movement.MaxSpeed + target.Movement.Speed);
        return Flee(target.Pos + target.Movement.Velocity * LookAheadTime);
    }

    Vector2 Wander()
    {
        m_vWanderTarget += Random.insideUnitCircle * m_WanderJitter;
        m_vWanderTarget = m_vWanderTarget.normalized * m_WanderRadius;
        Vector2 targetLocal = m_vWanderTarget + new Vector2(m_WanderDistance, 0);
        Vector2 targetWorld = yMath.PointToWorldSpace(targetLocal, m_Entity.Movement.Heading, m_Entity.Movement.Side, m_Entity.Movement.Pos);
        return targetWorld - m_Entity.Movement.Pos;
    }

    Vector2 ObstacleAvoidance(List<Obstacle> oList)
    {
        float dBoxLength = Config.MinDetectionBoxLength + (m_Entity.Movement.Speed / m_Entity.Movement.MaxSpeed) * Config.MinDetectionBoxLength;
        Obstacle ClosestIntersectingObstacle = null;
        float DistToClosestIP = Config.ObstacleMaxDistance;
        Vector2 LocalPosOfClosestObstacle = Vector2.zero;
        foreach (Obstacle obstacle in oList)
        {
            Vector2 LocalPos = yMath.PointToLocalSpace(obstacle.Pos, m_Entity.Movement.Heading, m_Entity.Movement.Side, m_Entity.Movement.Pos);
            
            if (LocalPos.x >= 0)
            {
                float ExpandedRadius = m_Entity.BRadius + obstacle.BRadius;
                if (Mathf.Abs(LocalPos.y) < ExpandedRadius)
                {
                    float cX = LocalPos.x;
                    float cY = LocalPos.y;
                    float SqrtPart = Mathf.Sqrt(ExpandedRadius * ExpandedRadius - cY * cY);
                    float ip = cX - SqrtPart;
                    if (ip <= 0)
                    {
                        ip = cX + SqrtPart;
                    }

                    if (ip < DistToClosestIP)
                    {
                        DistToClosestIP = ip;
                        ClosestIntersectingObstacle = obstacle;
                        LocalPosOfClosestObstacle = LocalPos;

                    }
                }
            }
        }
        Vector2 SteeringForce = Vector2.zero;
        if (ClosestIntersectingObstacle)
        {
            float multiplier = 1 + (dBoxLength - LocalPosOfClosestObstacle.x) / dBoxLength;
            SteeringForce.y = (ClosestIntersectingObstacle.BRadius - LocalPosOfClosestObstacle.y) * multiplier;
            SteeringForce.x = (ClosestIntersectingObstacle.BRadius - LocalPosOfClosestObstacle.x) * m_BrakingWeight;
            /*
            Vector3 pos = m_Entity.transform.position;
            Vector3 f1 = new Vector3(ClosestIntersectingObstacle.Pos().x, 0, ClosestIntersectingObstacle.Pos().y);
            Vector2 sfw = yMath.VectorToWorldSpace(SteeringForce, m_Entity.Movement.Heading(), m_Entity.Movement.Side());
            Vector3 f2 = pos + new Vector3(sfw.x, 0, sfw.y);
            Debug.DrawLine(pos, f1);
            Debug.DrawLine(pos, f2);*/
        }

        return yMath.VectorToWorldSpace(SteeringForce, m_Entity.Movement.Heading, m_Entity.Movement.Side);
    }

    Vector2 WallAvoidance(List<Wall> walls)
    {
        feelers.Clear();
        //Feeler0
        feelers.Add(yMath.PointToWorldSpace(feeleroffset0, m_Entity.Movement.Heading, m_Entity.Movement.Side, m_Entity.Movement.Pos));
        
        //Feeler1
        feelers.Add(yMath.PointToWorldSpace(feeleroffset1, m_Entity.Movement.Heading, m_Entity.Movement.Side, m_Entity.Movement.Pos));

        /*
        Vector3 pos = m_Entity.transform.position;
        Vector3 f1 = new Vector3(feelers[0].x, 0, feelers[0].y);
        Vector3 f2 = new Vector3(feelers[1].x, 0, feelers[1].y);
        Debug.DrawLine(pos, f1);
        Debug.DrawLine(pos, f2);
        */

        float DistToThisIp = 0;
        float DistToClosestIp = Config.WallMaxDistance;

        int ClosestWall = -1;
        Vector2 SteeringForce = Vector2.zero;
        Vector2 point = Vector2.zero;
        Vector2 ClosestPoint = Vector2.zero;

        foreach (Vector2 feeler in feelers)
        {
            for(int w=0; w< walls.Count;++w)
            {
                if (yMath.LineIntersection2D(m_Entity.Movement.Pos, feeler, walls[w].From(), walls[w].To(), out DistToThisIp, out point))
                {
                    if (DistToThisIp < DistToClosestIp)
                    {
                        DistToClosestIp = DistToThisIp;
                        ClosestWall = w;
                        ClosestPoint = point;
                    }
                }
            }
            if (ClosestWall >= 0)
            {
                Vector2 OverShoot = feeler - ClosestPoint;
                SteeringForce = walls[ClosestWall].Normal() * OverShoot.magnitude;
            }
        }
        
        return SteeringForce;
    }

    Vector2 OffsetPursuit(Character leader, Vector2 offset)
    {
        Vector2 WorldOffsetPos = yMath.PointToWorldSpace(offset, leader.Movement.Heading, leader.Movement.Side, leader.Pos);
        Vector2 ToOffset = WorldOffsetPos - m_Entity.Movement.Pos;
        float LookAheadTime = ToOffset.magnitude / (m_Entity.Movement.MaxSpeed + leader.Movement.Speed);
        return Arrive(WorldOffsetPos + leader.Movement.Velocity * LookAheadTime);
    }

    Vector2 Separation(List<BaseEntity> neighbors)
    {
        Vector2 SteeringForce = Vector2.zero;
        foreach (BaseEntity neighbor in neighbors)
        {
            if ((neighbor != m_Entity) && (neighbor.IsTagged) && (neighbor.EType == EntityType.Character))
            {
                Vector2 ToAgent = m_Entity.Pos - neighbor.Pos;
                SteeringForce += ToAgent.normalized / ToAgent.magnitude;
            }
        }
        return SteeringForce;
    }

    Vector2 Alignment(List<BaseEntity> neighbors)
    {
        int NeighborCount = 0;
        Vector2 AverageHeading = Vector2.zero;
        foreach (BaseEntity neighbor in neighbors)
        {
            if ((neighbor != m_Entity) && (neighbor.IsTagged) && (neighbor.EType == EntityType.Character))
            {
                Character n = (Character)neighbor;
                AverageHeading += n.Movement.Heading;
                NeighborCount++;
            }
            
        }
        if (NeighborCount > 0)
        {
            AverageHeading /= (float)NeighborCount;
            AverageHeading -= m_Entity.Movement.Heading;
        }
        return AverageHeading;
    }

    Vector2 Cohesion(List<BaseEntity> neighbors)
    {
        Vector2 CenterOfMass = Vector2.zero;
        Vector2 SteeringForce = Vector2.zero;
        int NeighborCount = 0;
        foreach (BaseEntity neighbor in neighbors)
        {
            if ((neighbor != m_Entity) && (neighbor.IsTagged) && (neighbor.EType == EntityType.Character))
            {
                CenterOfMass += neighbor.Pos;
                NeighborCount++;
            }
        }
        
        if (NeighborCount > 0)
        {
            CenterOfMass /= (float)NeighborCount;
            SteeringForce = Seek(CenterOfMass);
        }
        return SteeringForce;
    }

    Vector2 Crowd(List<BaseEntity> neighbors)
    {
        int crowdCount = 0;
        Vector2 SteeringForce = Vector2.zero;
        foreach (BaseEntity neighbor in neighbors)
        {
            if (neighbor.EType == EntityType.Character)
            {
                if ((neighbor != m_Entity) && (neighbor.IsTagged))
                {
                    Vector2 ToAgent = m_Entity.Pos - neighbor.Pos;
                    if (ToAgent.magnitude < Config.CrowdlMaxDistance)
                        ++crowdCount;
                }
            }
        }

        SteeringForce = -m_Entity.Movement.Heading * (crowdCount);
        return SteeringForce;
    }

    public void TagNeighbors(Character entity, List<BaseEntity> ContainerOfEntities, float radius)
    {
        foreach (BaseEntity e in ContainerOfEntities)
        {
            if (!e.IsStatic)
            {
                if (e.HitTest(entity.Pos, radius))
                {
                    e.IsTagged = true;
                }
            }
        }
    }

    bool AccumulateForce(ref Vector2 total, Vector2 ForceToAdd)
    {
        float remain = m_Entity.Movement.MaxForce - total.magnitude;
        if (remain <= 0) return false;
        float MagnitudeToAdd = ForceToAdd.magnitude;
        if (MagnitudeToAdd < remain)
        {
            total += ForceToAdd;
        }
        else
        {
            total += ForceToAdd.normalized * remain;
        }
        return true;
    }

    public Vector2 Calculate()
    {
        if (m_Separation || m_Alignment || m_Cohesion || m_Crowd)
        {
            TagNeighbors(m_Entity, m_Entity.World.Partition.Neighbors(), m_ViewDistance);
        }

        m_SteeringForce = Vector2.zero;
        Vector2 force = Vector2.zero;

        if (m_WallAvoidance)
        {
            force = WallAvoidance(m_Entity.World.Walls()) * m_dMultWallAvoidance;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        if (m_ObstacleAvoidance)
        {
            force = ObstacleAvoidance(m_Entity.World.Obstacles()) * m_dMultObstacleAvoidance;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        if (m_Separation)
        {
            force = Separation(m_Entity.World.Partition.Neighbors()) * m_dMultSeparation;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        if (m_Alignment)
        {
            force = Alignment(m_Entity.World.Partition.Neighbors()) * m_dMultAlignment;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        if (m_Cohesion)
        {
            force = Cohesion(m_Entity.World.Partition.Neighbors()) * m_dMultCohesion;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        if (m_Crowd)
        {
            force = Crowd(m_Entity.World.Partition.Neighbors()) * m_dMultCrowd;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }

        if (m_Seek)
        {
            force = Seek(m_Target) * m_dMultSeek;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        //Debug.Log(m_SteeringForce);
        if (m_Arrive)
        {
            force = Arrive(m_Target) * m_dMultArrive;
            //Debug.Log(force);
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        //Debug.Log(m_SteeringForce);
        //Debug.Log(m_SteeringForce);
        if (m_Pursuit)
        {
            force = Pursuit(m_Entity.Movement.Target) * m_dMultPursuit;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }

        if (m_OffsetPursuit)
        {
            force = OffsetPursuit(m_Entity.Movement.Target, m_Entity.Movement.TeamOffset) * m_dMultPursuit;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }

        if (m_Evade)
        {
            force = Evade(m_Entity.Movement.Target) * m_dMultEvade;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }

        if (m_Wander)
        {
            force = Wander() * m_dMultWander;
            if (!AccumulateForce(ref m_SteeringForce, force)) return m_SteeringForce;
        }
        //Debug.Log(m_SteeringForce);


        return m_SteeringForce;
    }
}
