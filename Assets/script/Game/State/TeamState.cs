using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamGlobalState : State<Minor>
{
    static TeamGlobalState m_Instance;
    public static TeamGlobalState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new TeamGlobalState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Minor entity)
    {
    }
    public override void Execute(Minor entity)
    {
    }
    public override void Exit(Minor entity)
    {
    }

    public override bool OnMessage(Minor entity, Telegram msg)
    {
        bool result = false;

        switch (msg.Msg)
        {
            default:
                break;
            case MessageType.Msg_Damage:
                entity.Health -= (int)msg.ExtraInfo;
                if (entity.Health <= 0)
                    //entity.FSM.ChangeState(MinorGoneState.Instance);
                result = true;
                break;

        }
        return result;
    }
}

public class TeamOffenceState : State<Minor>
{
    static TeamOffenceState m_Instance;
    public static TeamOffenceState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new TeamOffenceState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Minor entity)
    {
    }
    public override void Execute(Minor entity)
    {
    }
    public override void Exit(Minor entity)
    {
    }

    public override bool OnMessage(Minor entity, Telegram msg)
    {
        return false;
    }
}

public class TeamDefenceState : State<Minor>
{
    static TeamDefenceState m_Instance;
    public static TeamDefenceState Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new TeamDefenceState();
            }
            return m_Instance;
        }
    }
    public override void Enter(Minor entity)
    {
    }
    public override void Execute(Minor entity)
    {
    }
    public override void Exit(Minor entity)
    {
    }

    public override bool OnMessage(Minor entity, Telegram msg)
    {
        return false;
    }
}