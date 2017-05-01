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
                    entity.Target = pInfo.Shooter;
                    entity.Health -= pInfo.Damage;
                }
                break;
            case MessageType.Msg_Push:
                pInfo = (ProjectileExtraInfo)msg.ExtraInfo;
                if (entity.IsEnemy(pInfo.Shooter))
                {
                    entity.Target = pInfo.Shooter;
                    entity.Health -= pInfo.Damage;
                }
                break; 

        }
        return result;
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
        entity.MaxSpeed = Config.MaxSpeedJump;
        entity.OnGround = false;
        entity.JumpVelocity = 2.5f;
        Vector2 JumpTo = (entity.Steering.Target - entity.Pos).normalized;
        entity.Velocity = JumpTo * entity.MaxSpeed;
        entity.IsNonPenetrationConstraint = false;
    }
    public override void Execute(Character entity)
    {
        entity.JumpVelocity += Config.Gravity * Time.deltaTime;
        entity.Height += entity.JumpVelocity;
        if (entity.Height <= 0)
        {
            entity.Height = 0;
            if(entity.FSM.PreviousState()==this)
                entity.FSM.ChangeState(MinorMovingState.Instance);
            entity.FSM.RevertToPreviousState();
        }
    }

    public override void Exit(Character entity)
    {
        entity.Height = 0;
        entity.IsNonPenetrationConstraint = true;
        entity.OnGround = true;
        entity.MaxSpeed = 0;
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());

    }
    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        if (m.IsNonPenetrationConstraint)
        {
            foreach (BaseEntity curEntity in ContrainerOfEntities)
            {
                if (curEntity != m && curEntity.IsNonPenetrationConstraint)
                {
                    Vector2 ToEntity = m.Pos - curEntity.Pos;
                    float DistFromEachOther = ToEntity.magnitude;
                    if (DistFromEachOther >= 0.001f)
                    {
                        float AmountOfOverLap = m.BRadius + curEntity.BRadius - DistFromEachOther;
                        if (AmountOfOverLap >= 0)
                        {
                            m.Pos += (ToEntity / DistFromEachOther) * AmountOfOverLap;
                        }
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
        entity.MaxSpeed = Config.MaxSpeedWander;
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
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());
    }

    public override void Exit(Character entity)
    {
        entity.MaxSpeed = 0;
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
        entity.MaxSpeed = Config.MaxSpeedMoving;
        entity.Attention = Config.TimeToStandUpSlow;
        entity.Steering.IsArrive = true;
    }
    public override void Execute(Character entity)
    {
        --entity.Attention;
        if (entity.Attention <= 0)
            entity.FSM.ChangeState(MinorIdleState.Instance);

        if (entity.Steering.HasArrived())
            entity.FSM.ChangeState(MinorDefenceState.Instance);

        Team t = entity.World.GetOpponent(entity.Team);
        if (entity.Weapon.FindEnemy(t.Members()))
            if (entity.Weapon.AimAt(entity.Target.Pos, false))
                entity.Weapon.ShootAt((Character)entity.Target);
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());
    }
    public override void Exit(Character entity)
    {
        entity.Steering.IsArrive = false;
        entity.MaxSpeed = 0;
    }

    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        if (m.IsNonPenetrationConstraint)
        {
            foreach (BaseEntity curEntity in ContrainerOfEntities)
            {
                if (curEntity != m && curEntity.IsNonPenetrationConstraint)
                {
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
                    }
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
        entity.MaxSpeed = 0;
        //entity.Attention = Config.TimeToStandUpSlow;
        
    }
    public override void Execute(Character entity)
    {
        Team t = entity.World.GetOpponent(entity.Team);
        if (entity.Weapon.FindEnemy(t.Members()))
            if (entity.Weapon.AimAt(entity.Target.Pos, false))
                entity.Weapon.ShootAt((Character)entity.Target);
        /*
        --entity.Attention;
        if (entity.Attention <= 0)
            entity.FSM.ChangeState(MinorIdleState.Instance);
        */

    }
    public override void Exit(Character entity)
    {

    }
}

