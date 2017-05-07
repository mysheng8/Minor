using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharType
{
    MinorSpear,
    MinorMagic,
    MinorKnife,
    Superior,
}

public class TeamDesc
{
    public int Health;
    public float Radius;
    public int Num;
    public WeaponType WType;
    public TeamDesc(int health, float radius, int num, WeaponType wtype)
    {
        Health = health;
        Radius = radius;
        Num = num;
        WType = wtype;
    }
}

public class TeamStruct
{
    public Dictionary<CharType, TeamDesc> TeamDict;//1:char type, 2: num, 3: health 
    public TeamStruct()
    {
        TeamDict = new Dictionary<CharType, TeamDesc>();
    }
}

public class Team 
{
    
    protected List<Character> m_Members;

    public List<Character> Members()
    {
        return m_Members;
    }
    protected TeamStruct m_Struct;
    GameWorld m_World;
    int m_Count;
    CharacterSpawner m_Spawner;

    public int Count
    {
        get
        {
            return m_Count;
        }
        set
        {
            m_Count = value;
        }
    }

    public GameWorld World
    {
        get
        {
            return m_World;
        }
    }

    public CharacterSpawner Spawner
    {
        get
        {
            return m_Spawner;
        }
        set
        {
            m_Spawner = value;
        }
    }

    public Team(TeamStruct teamStruct, int count)
    {
        m_Members = new List<Character>();
        m_World = GameWorld.Instance;
        m_Count = count;
        m_Struct = teamStruct;
	}
    public virtual void OnStart()
    {
        m_Spawner.CreateTeam(this, m_Struct);
    }

    public virtual void OnUpdate() 
    {
        m_Spawner.UpdateTeam(this, m_Struct);
	}


}



public class MinorTeam : Team
{
    MinorTeamFormat m_MTF;
    public MinorTeam(TeamStruct teamStruct, int count)
        : base(teamStruct, count)
    {
        m_MTF = LineMinorTeamFormat.Instance;
    }

    public void SetMTF(TeamFormationType tType)
    {
        switch (tType)
        {
            default:
                break;
            case TeamFormationType.Line:
                m_MTF = LineMinorTeamFormat.Instance;
                break;
            case TeamFormationType.Wall:
                m_MTF = WallMinorTeamFormat.Instance;
                break;
            case TeamFormationType.Ball:
                m_MTF = BallMinorTeamFormat.Instance;
                break;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public void RemoveMinor(Character ent)
    {
        World.Partition.RemoveEntity((BaseEntity)ent, ent.LastPosInCellSpace);
        GameObject.Destroy(ent.gameObject, 0);
        --m_Struct.TeamDict[ent.CType].Num;
        m_Members.Remove(ent);
    }

    public Vector2 GetMoveTarget()
    {
        return m_MTF.GetMoveTarget();
    }

    void OnControl()
    {
        m_MTF.OnMouseUpdate(m_Struct,m_Members);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        OnControl();

    }
}

public class SuperiorTeam : Team
{

    public SuperiorTeam(TeamStruct teamStruct, int count)
        : base(teamStruct, count)
    {

    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
