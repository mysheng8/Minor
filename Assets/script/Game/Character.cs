using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MovingEntity
{
    CharType m_CType;

    int m_Health = Config.DefaultHealth;
    int m_Attention = Config.TimeToClamDown;
    float m_Height = 0;
    float m_JumpVelocity = 0;
    Team m_Team;
    StateMachine<Character> m_StateMachine;
    Weapon m_Weapon;
    float random;

    public float Height
    {
        get
        {
            return m_Height;
        }
        set
        {
            m_Height = value;
        }
    }

    public float JumpVelocity
    {
        get
        {
            return m_JumpVelocity;
        }
        set
        {
            m_JumpVelocity = value;
        }
    }

    public CharType CType
    {
        get
        {
            return m_CType;
        }
    }


    public StateMachine<Character> FSM
    {
        get
        {
            return m_StateMachine;
        }
    }

    public Weapon Weapon
    {
        get
        {
            return m_Weapon;
        }
        set
        {
            m_Weapon = value;
        }
    }

    public Team Team
    {
        get
        {
            return m_Team;
        }
        set
        {
            m_Team = value;
        }
    }

    public int Health
    {
        get
        {
            return m_Health;
        }
        set
        {
            m_Health = value;
        }
    }

    public int Attention
    {
        get
        {
            return m_Attention;
        }
        set
        {
            m_Attention = value;
        }
    }

    public bool IsEnemy(Character ent)
    {
        return ent.Team != Team;
    }

    public void Init(CharType ctype, int health, float radius, Team team, WeaponType wtype)
    {
        m_CType = ctype;
        Team = team;
        BRadius = radius;
        Health = health;
        Weapon = new Weapon(wtype, this);
        team.Members().Add(this);
    }

    // Use this for initialization
    public void Awake()
    {
        base.Awake();
        m_StateMachine = new StateMachine<Character>(this);
        random = Random.value / 2;
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        
        m_StateMachine.Update();
        
        //float h = Mathf.Abs(Mathf.Sin((Time.realtimeSinceStartup + random) * 20.0f) * 2.0f) + 1;
        
        gameObject.transform.position = new Vector3(m_Pos.x, m_Height, m_Pos.y);

    }

    public float GetGroundHeight()
    {
        return World.GetHeight(Pos);
    }

    public override bool HandleMessage(Telegram msg)
    {
        return m_StateMachine.HandleMessage(msg);
    }
}
