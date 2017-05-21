using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : BaseEntity
{
    CharType m_CType;

    int m_Health = Config.DefaultHealth;
    int m_Attention = Config.TimeToClamDown;

    Team m_Team;
    Weapon m_Weapon;

    StateMachine<Character> m_StateMachine;
    FreeMovement m_Movement;
    SteeringBehaviors m_Steering;


    //jump parameter
    float m_JumpVelocity;
    bool m_OnGround = true;

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
    public bool OnGround
    {
        get
        {
            return m_OnGround;
        }
        set
        {
            m_OnGround = value;
        }
    }

    float random;

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
    public SteeringBehaviors Steering
    {
        get
        {
            return m_Steering;
        }
    }


    public FreeMovement Movement
    {
        get
        {
            return m_Movement;
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
        //need to update cell space
        IsStatic = false;
        EType = EntityType.Character;
        Pos = new Vector2(transform.position.x, transform.position.z);
        m_StateMachine = new StateMachine<Character>(this);
        m_Movement = new FreeMovement(this);
        m_Steering = m_Movement.Steering;
        random = Random.value / 2;
    }

    // Update is called once per frame
    public void Update()
    {
        base.Update();
        
        m_Movement.UpdateTransform();
        m_StateMachine.Update();
        //Debug.Log(GetInstanceID() + " pos: " + Pos + " states: " + (m_StateMachine.CurrentState()));
        gameObject.transform.position = m_Movement.GetPosition();
        gameObject.transform.rotation = m_Movement.GetRotation();
        
        
    }

    public float GetGroundHeight()
    {
        return World.GetHeight(Pos);
    }

    public Vector3 GetWorldPosition()
    {
        return m_Movement.GetPosition();
    }

    public override bool HandleMessage(Telegram msg)
    {
        return m_StateMachine.HandleMessage(msg);
    }

    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        return yMath.CircleHitTest(Pos, BRadius, entityPos, entityRadius);
    }

    public override Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        return yMath.CalculateCircleOverlay(Pos, BRadius, entityPos, entityRadius);
    }

}
