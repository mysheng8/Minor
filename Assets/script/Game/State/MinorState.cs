//#define DEBUGMODE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MinorGlobalState : State<Character>
{
    static MinorGlobalState m_Instance;
    public static MinorGlobalState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorGlobalState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
        entity.Steering.IsWallAvoidance = true;
        entity.Steering.IsObstacleAvoidance = true;
        entity.Steering.IsSeparation = true;
        //entity.Steering.IsCrowd = true;
    }
    public override void Execute(Character entity)
    {
        if (entity.Health <= 0)
        {
            MinorTeam t = (MinorTeam)entity.Team;
            t.RemoveMinor(entity);

        }
        EnforceNonOutWallConstraint(entity, entity.World.Walls());
    }
    public override void Exit(Character entity)
    {
        entity.Steering.IsWallAvoidance = false;
        entity.Steering.IsObstacleAvoidance = false;
        entity.Steering.IsSeparation = false;
        //entity.Steering.IsCrowd = false;
    }

    public override bool OnMessage(Character entity, Telegram msg)
    {
        bool result = false;
        ProjectileExtraInfo pInfo;
        switch (msg.Msg)
        { 
            default:
                break;
            case MessageType.Msg_Damage:
                pInfo = (ProjectileExtraInfo)msg.ExtraInfo;
                if (entity.IsEnemy(pInfo.Shooter))
                {
                    entity.Movement.Target = pInfo.Shooter;
                    entity.Health -= pInfo.Damage;
                }
                break;
            case MessageType.Msg_Push:
                pInfo = (ProjectileExtraInfo)msg.ExtraInfo;
                if (entity.IsEnemy(pInfo.Shooter))
                {
                    entity.Movement.Target = pInfo.Shooter;
                    entity.Health -= pInfo.Damage;
                }
                break; 

        }
        return result;
    }
    void EnforceNonOutWallConstraint(Character m, List<Wall> walls)
    {
        foreach (Wall w in walls)
        {
            Vector2 dir = m.Movement.Pos - w.From();
            float dis = yMath.DistPointToLine2D(m.Movement.Pos, w.From(), w.To());
            //Debug.Log(m.GetInstanceID() + "Distance To Wall = " + dis + "; Current Pos = " + m.Movement.Pos);
            if (dis > 0 && dis < Config.MinDetectionWallDistance)
            {
                float AmountOfOverLap;
                if (Vector2.Dot(w.Normal(), dir) > 0)
                    AmountOfOverLap = m.BRadius + Config.WallThickness - dis;
                else
                    AmountOfOverLap = m.BRadius + Config.WallThickness + dis;

                //Debug.Log("[" + Time.time + "]" + m.GetInstanceID() + m.Pos + dis + AmountOfOverLap);
                if (AmountOfOverLap > 0)
                {
                    Vector2 move = w.Normal() * (AmountOfOverLap);
                    //Debug.Log("["+ Time.time + "]"+ m.GetInstanceID() + m.Pos + move +  dis +  AmountOfOverLap);
                    m.Movement.Pos += move;
                }
            }
        }
    }

}
public class MinorJumpState : State<Character>
{
    static MinorJumpState m_Instance;
    public static MinorJumpState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorJumpState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Enter " + this);
#endif
        entity.Movement.MaxSpeed = Config.MaxSpeedJump;
        entity.OnGround = false;
        entity.JumpVelocity = 2.5f;
        Vector2 JumpTo = entity.Movement.Heading;//(entity.Steering.Target - entity.Pos).normalized;
        entity.Movement.Velocity = JumpTo * entity.Movement.MaxSpeed;
        //entity.IsNonPenetrationConstraint = false;
        
    }
    public override void Execute(Character entity)
    {
        entity.JumpVelocity += Config.Gravity * Time.deltaTime;

        float ground_height = entity.GetGroundHeight();
        entity.Movement.Height += entity.JumpVelocity;
        if (entity.Movement.Height <= ground_height && entity.JumpVelocity < 0)
        {
            entity.Movement.Height = ground_height;
            if (entity.FSM.PreviousState() == this)
                entity.FSM.ChangeState(MinorIdleState.Instance);
            if (entity.FSM.PreviousState() == MinorBigJumpState.Instance)
                entity.FSM.ChangeState(MinorIdleState.Instance);
            entity.FSM.RevertToPreviousState();
        }
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());
    }

    public override void Exit(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Exit " + this);
#endif
        entity.Movement.Height = entity.GetGroundHeight();
        //entity.IsNonPenetrationConstraint = true;
        entity.OnGround = true;
        entity.Movement.MaxSpeed = 0;
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());

    }
    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        if (m.IsNonPenetrationConstraint)
        {
            foreach (BaseEntity curEntity in ContrainerOfEntities)
            {
                if (curEntity != m && curEntity.IsNonPenetrationConstraint && !curEntity.IsJumpable)
                {

                    Vector2 pushOffset = curEntity.CalculatePenetrationConstraint(m.Movement.Pos, m.BRadius);
                    
                    Vector3 pos = m.Movement.GetPosition();
                    Vector3 posMove = new Vector3(pushOffset.x, 0, pushOffset.y);
                    Debug.DrawLine(pos, pos + posMove);
                    if (!curEntity.IsJumpable && !m.OnGround)
                    {
                        m.Movement.Pos += pushOffset;
                    }
                    else if (m.OnGround)
                    {
                        m.Movement.Pos += pushOffset;
                    }
                }
            }
        }
    }
}

public class MinorBigJumpState : State<Character>
{
    static MinorBigJumpState m_Instance;
    public static MinorBigJumpState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorBigJumpState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Enter " + this);
#endif
        entity.Movement.MaxSpeed = Config.MaxSpeedJump;
        entity.OnGround = false;
        entity.JumpVelocity = 3.5f;
        Vector2 JumpTo = (entity.Steering.Target - entity.Pos).normalized;
        entity.Movement.Velocity = JumpTo * entity.Movement.MaxSpeed;
        //entity.IsNonPenetrationConstraint = false;
    }
    public override void Execute(Character entity)
    {
        entity.JumpVelocity += Config.Gravity * Time.deltaTime;

        float ground_height = entity.GetGroundHeight();
        entity.Movement.Height += entity.JumpVelocity;
        if (entity.Movement.Height <= ground_height && entity.JumpVelocity < 0)
        {
            entity.Movement.Height = ground_height;
            if (entity.FSM.PreviousState() == this)
                entity.FSM.ChangeState(MinorIdleState.Instance);
            if (entity.FSM.PreviousState() == MinorJumpState.Instance)
                entity.FSM.ChangeState(MinorIdleState.Instance);
            entity.FSM.RevertToPreviousState();
        }
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());
    }

    public override void Exit(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Exit " + this);
#endif
        entity.Movement.Height = entity.GetGroundHeight();
        //entity.IsNonPenetrationConstraint = true;
        entity.OnGround = true;
        entity.Movement.MaxSpeed = 0;
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());

    }
    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        if (m.IsNonPenetrationConstraint)
        {
            foreach (BaseEntity curEntity in ContrainerOfEntities)
            {
                if (curEntity != m && curEntity.IsNonPenetrationConstraint && !curEntity.IsJumpable)
                {
                    Vector2 pushOffset = curEntity.CalculatePenetrationConstraint(m.Movement.Pos, m.BRadius);
                    /*
                    Vector3 pos = m.Movement.GetPosition();
                    Vector3 posMove = new Vector3(pushOffset.x, 0, pushOffset.y);
                    Debug.DrawLine(pos, pos + posMove);*/
                    if (!curEntity.IsJumpable && !m.OnGround)
                    {
                        m.Movement.Pos += pushOffset;
                    }
                    else if (m.OnGround)
                    {
                        m.Movement.Pos += pushOffset;
                    }
                }
            }
        }
    }
}

public class MinorIdleState : State<Character>
{
    static MinorIdleState m_Instance;
    public static MinorIdleState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorIdleState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Enter " + this);
#endif
        entity.Movement.MaxSpeed = Config.MaxSpeedWander;
        entity.Steering.IsWander = true;
        entity.Steering.IsAlignment = true;
        entity.Steering.IsCohesion = true;

    }

    public override void Execute(Character entity)
    {
        /*
        Team t = entity.World.GetOpponent(entity.Team);

        if (entity.Weapon.FindEnemy(t.Members()))
            entity.FSM.ChangeState(MinorAttackState.Instance);*/
        entity.Height = entity.GetGroundHeight();
        float deltaHeight = entity.World.GetHeight(entity.Pos + entity.Movement.Heading * Config.MinDetectionJumpDistance) - entity.Movement.Height;
        if (deltaHeight > 10)
        {
#if DEBUGMODE
            Debug.Log("[" + Time.time + "]" + entity.GetInstanceID() + "; Current Pos = " + entity.Pos + "; Jump " + deltaHeight);
#endif
            entity.FSM.ChangeState(MinorBigJumpState.Instance);
        }
        if (deltaHeight < -10)
        {
#if DEBUGMODE
            Debug.Log("[" + Time.time + "]" + entity.GetInstanceID() + "; Current Pos = " + entity.Pos + "; Jump " + deltaHeight);
#endif
            entity.FSM.ChangeState(MinorJumpState.Instance);
        }
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());
    }

    public override void Exit(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Exit " + this);
#endif
        entity.Movement.MaxSpeed = 0;
        entity.Steering.IsWander = false;
        entity.Steering.IsAlignment = false;
        entity.Steering.IsCohesion = false;
    }

    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        if (m.IsNonPenetrationConstraint)
        {
            foreach (BaseEntity curEntity in ContrainerOfEntities)
            {
                if (curEntity != m && curEntity.IsNonPenetrationConstraint)
                {
                    Vector2 pushOffset = curEntity.CalculatePenetrationConstraint(m.Movement.Pos, m.BRadius);
                    /*
                    Vector3 pos = m.Movement.GetPosition();
                    Vector3 posMove = new Vector3(pushOffset.x, 0, pushOffset.y);
                    Debug.DrawLine(pos, pos + posMove);*/
                    if (pushOffset.magnitude > 0)
                    {
						if (curEntity.IsJumpable&&m.Movement.Speed>Config.MaxSpeedJumpThreshold)
                        {
                            m.FSM.ChangeState(MinorJumpState.Instance);
                        }
                        else
                        {
                            m.Movement.Pos += pushOffset;
                        }
                    }
                }
            }
        }
    }

}


/*
public class MinorAttackState : State<Character>
{
    static MinorAttackState m_Instance;
    public static MinorAttackState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorAttackState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
        entity.MaxSpeed = Config.MaxSpeedAttention;
        entity.Attention = Config.TimeToClamDown;
    }
    public override void Execute(Character entity)
    {
        if (entity.Weapon.AimAt(entity.Target.Pos,true) )
            entity.Weapon.ShootAt((Character)entity.Target);

        entity.Attention -= 1;

        if (((Character)entity.Target).Health <= 0 || entity.Attention <= 0)
            entity.FSM.ChangeState(MinorIdleState.Instance);

    }
}*/

public class MinorMovingState : State<Character>
{
    static MinorMovingState m_Instance;
    public static MinorMovingState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorMovingState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Enter " + this);
#endif
        entity.Movement.MaxSpeed = Config.MaxSpeedMoving;
        entity.Attention = Config.TimeToStandUpSlow;
        entity.Steering.IsArrive = true;
    }
    public override void Execute(Character entity)
    {

        entity.Movement.Height = entity.GetGroundHeight();
        float deltaHeight = entity.World.GetHeight(entity.Pos + entity.Movement.Heading * Config.MinDetectionJumpDistance) - entity.Movement.Height;
        if (deltaHeight > 10)
        {
#if DEBUGMODE
            Debug.Log("[" + Time.time + "]" + entity.GetInstanceID() + "; Current Pos = " + entity.Pos + "; Jump " + deltaHeight);
#endif
            entity.FSM.ChangeState(MinorBigJumpState.Instance);
        }
        if (deltaHeight < -10)
        {
#if DEBUGMODE
            Debug.Log("[" + Time.time + "]" + entity.GetInstanceID() + "; Current Pos = " + entity.Pos + "; Jump " + deltaHeight);
#endif
            entity.FSM.ChangeState(MinorJumpState.Instance);
        }
        --entity.Attention;
        if (entity.Attention <= 0)
            entity.FSM.ChangeState(MinorIdleState.Instance);
        /*
        if (entity.Steering.HasArrived())
            entity.FSM.ChangeState(MinorDefenceState.Instance);*/

        Team t = entity.World.GetOpponent(entity.Team);
        if (entity.Weapon.FindEnemy(t.Members()))
            if (entity.Weapon.AimAt(entity.Movement.Target.Pos, false))
                entity.Weapon.ShootAt((Character)entity.Movement.Target);
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());
    }
    public override void Exit(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Exit " + this);
#endif
        entity.Steering.IsArrive = false;
        entity.Movement.MaxSpeed = 0;
    }

    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        if (m.IsNonPenetrationConstraint)
        {
            foreach (BaseEntity curEntity in ContrainerOfEntities)
            {
                if (curEntity != m && curEntity.IsNonPenetrationConstraint)
                {
                    Vector2 pushOffset = curEntity.CalculatePenetrationConstraint(m.Movement.Pos, m.BRadius);
                    
                    Vector3 pos = m.Movement.GetPosition();
                    Vector3 posMove = new Vector3(pushOffset.x, 0, pushOffset.y);
                    Debug.DrawLine(pos, pos + posMove*10);

                    if (pushOffset.magnitude > 0)
                    {
						if (curEntity.IsJumpable&&m.Movement.Speed>Config.MaxSpeedJumpThreshold)
                        {
                            m.FSM.ChangeState(MinorJumpState.Instance);
                        }
                        else
                        {
                            m.Movement.Pos += pushOffset;
                        }
                    }
                    /*
                    Vector2 ToEntity = m.Pos - curEntity.Pos;
                    float DistFromEachOther = ToEntity.magnitude;
                    if (DistFromEachOther >= 0.001f)
                    {
                        float AmountOfOverLap = m.BRadius + curEntity.BRadius - DistFromEachOther;
                        if (AmountOfOverLap >= 0)
                        {
                            
                            if (curEntity.IsMovingEntity())
                                m.FSM.ChangeState(MinorJumpState.Instance);
                            else
                                m.Pos += (ToEntity / DistFromEachOther) * AmountOfOverLap;
                        }
                    }*/
                }
            }
        }
    }
}

public class MinorDefenceState : State<Character>
{
    static MinorDefenceState m_Instance;
    public static MinorDefenceState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new MinorDefenceState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Enter " + this);
#endif
        entity.Movement.MaxSpeed = 0;
        //entity.Attention = Config.TimeToStandUpSlow;
        
    }
    public override void Execute(Character entity)
    {
        Team t = entity.World.GetOpponent(entity.Team);
        if (entity.Weapon.FindEnemy(t.Members()))
            if (entity.Weapon.AimAt(entity.Movement.Target.Pos, false))
                entity.Weapon.ShootAt((Character)entity.Movement.Target);
        /*
        --entity.Attention;
        if (entity.Attention <= 0)
            entity.FSM.ChangeState(MinorIdleState.Instance);
        */
        entity.Movement.Height = entity.World.GetHeight(entity.Pos);
    }
    public override void Exit(Character entity)
    {
#if DEBUGMODE
        Debug.Log(entity.GetInstanceID() + " Exit " + this);
#endif
    }
}

