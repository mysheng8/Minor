using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperiorDefaultGlobalState : State<Character>
{
    static SuperiorDefaultGlobalState m_Instance;
    public static SuperiorDefaultGlobalState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SuperiorDefaultGlobalState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
        entity.Steering.IsWallAvoidance = true;
        entity.Steering.IsObstacleAvoidance = true;
        entity.Steering.IsSeparation = true;
    }
    public override void Execute(Character entity)
    {
        if (entity.Health <= 0)
            entity.FSM.ChangeState(SuperiorDefaultGoneState.Instance);
        else
        {
            if (entity.Health <= entity.Health / 2 )
                entity.FSM.ChangeState(SuperiorDefaultFleeState.Instance);
        }
        EnforceNonPenetrationConstraint(entity, entity.World.Partition.Neighbors());

    }
    public override void Exit(Character entity)
    {
        entity.Steering.IsWallAvoidance = false;
        entity.Steering.IsObstacleAvoidance = false;
        entity.Steering.IsSeparation = false;
    }

    public override bool OnMessage(Character entity, Telegram msg)
    {
        bool result = false;
        ProjectileExtraInfo pInfo;
        switch (msg.Msg)
        {

            case MessageType.Msg_Push:
                pInfo = (ProjectileExtraInfo)msg.ExtraInfo;
                if (entity.IsEnemy(pInfo.Shooter))
                {
                    Vector2 dir = (pInfo.Shooter.Pos - entity.Pos).normalized;
                    entity.Pos -= dir * pInfo.BackForward;
                    entity.Movement.Target = pInfo.Shooter;
                    entity.Health -= pInfo.Damage;
                }
                entity.FSM.ChangeState(SuperiorDefaultAttackState.Instance);
                //Debug.Log("Push");
                result = true;
                break;
            case MessageType.Msg_Damage:
                pInfo = (ProjectileExtraInfo)msg.ExtraInfo;
                if (entity.IsEnemy(pInfo.Shooter))
                {
                    entity.Movement.Target = pInfo.Shooter;
                    entity.Health -= pInfo.Damage;
                }
                entity.FSM.ChangeState(SuperiorDefaultAttackState.Instance);
                //Debug.Log("Damage");
                result = true;
                break;
            default:
                break;
        }
        return result;
    }

    void EnforceNonPenetrationConstraint(Character m, List<BaseEntity> ContrainerOfEntities)
    {
        foreach (BaseEntity curEntity in ContrainerOfEntities)
        {
            if (curEntity != m)
            {
                Vector2 pushOffset = curEntity.CalculatePenetrationConstraint(m.Pos, m.BRadius);
                m.Pos += pushOffset;
            }
        }
    }
}

public class SuperiorDefaultIdleState : State<Character>
{
    static SuperiorDefaultIdleState m_Instance;
    public static SuperiorDefaultIdleState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SuperiorDefaultIdleState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
        entity.Movement.MaxSpeed = Config.MaxSpeedWander;
        float FarGoal = 0;
        Vector2 GoalPos = Vector2.zero;
        foreach (Vector2 g in Config.ExitPositions())
        {
            float dis = (entity.Pos - g).magnitude;
            if (dis > FarGoal)
            {
                FarGoal = dis;
                GoalPos = g;
            }
        }
        entity.Steering.Target = GoalPos;
        entity.Steering.IsSeek = true;

    }
    public override void Execute(Character entity)
    {
        /*Team t = entity.World.GetOpponent(entity.Team);

        if (entity.Weapon.FindEnemy(t.Members()))
            entity.FSM.ChangeState(SuperiorDefaultAttackState.Instance);*/
    }
    public override void Exit(Character entity)
    {
        entity.Steering.IsSeek = false;

    }
}

public class SuperiorDefaultAttackState : State<Character>
{
    static SuperiorDefaultAttackState m_Instance;
    public static SuperiorDefaultAttackState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SuperiorDefaultAttackState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
        entity.Movement.MaxSpeed = Config.MaxSpeedAttention;
        entity.Attention = Config.TimeToClamDown;
    }
    public override void Execute(Character entity)
    {
        if (entity.Weapon.AimAt(entity.Movement.Target.Pos, true))
            entity.Weapon.ShootAt(entity.Movement.Target);

        entity.Attention -= 1;

        if (((Character)entity.Movement.Target).Health <= 0 || entity.Attention <= 0)
            entity.FSM.ChangeState(SuperiorDefaultIdleState.Instance);

    }
}

public class SuperiorDefaultFleeState : State<Character>
{
    static SuperiorDefaultFleeState m_Instance;
    public static SuperiorDefaultFleeState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SuperiorDefaultFleeState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Character entity)
    {
        entity.Movement.MaxSpeed = Config.MaxSpeedAttention;
        entity.Attention = Config.TimeToClamDown;
        
        entity.Steering.IsEvade = true;
    }

    public override void Execute(Character entity)
    {
        entity.Attention -= 1;
        if (entity.Attention <= 0)
            entity.FSM.ChangeState(SuperiorDefaultIdleState.Instance);
    }
    public override void Exit(Character entity)
    {
        entity.Steering.IsEvade = false;

    }

}


public class SuperiorDefaultGoneState : State<Character>
{
    static SuperiorDefaultGoneState m_Instance;
    public static SuperiorDefaultGoneState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new SuperiorDefaultGoneState();
            }
            return m_Instance;
        }
    }

    public override void Enter(Character entity)
    {
        entity.Movement.MaxSpeed = Config.MaxSpeedFlee;
        float closetGoal = 99999;
        Vector2 GoalPos = Vector2.zero;
        foreach (Vector2 g in Config.ExitPositions())
        {
            float dis = (entity.Pos - g).magnitude;
            if (dis < closetGoal)
            {
                closetGoal = dis;
                GoalPos = g;
            }
        }
        entity.Steering.Target = GoalPos;
        entity.Steering.IsSeek = true;

    }

}