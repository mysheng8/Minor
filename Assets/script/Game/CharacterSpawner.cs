using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterSpawner
{
    public virtual void CreateTeam(Team team, TeamStruct teamStruct) { }
    public virtual void UpdateTeam(Team team, TeamStruct teamStruct) { }

    public Minor SpawnMinor(Team team, CharType ctype, Vector2 spawnPos, float radius, WeaponType wtype, int health)
    {
        Vector3 pos = new Vector3(spawnPos.x, 0, spawnPos.y);
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        GameObject ent = (GameObject)Resources.Load(ctype.ToString());
        GameObject m = (GameObject)Object.Instantiate(ent, pos, rot);
        Minor c = m.AddComponent<Minor>();
        c.Init(ctype, health, radius, team, wtype);
        c.Awake();
        return c;
    }

    public Superior SpawnSuperior(Team team, CharType ctype, Vector2 spawnPos, float radius, WeaponType wtype, int health)
    {
        Vector3 pos = new Vector3(spawnPos.x, 0, spawnPos.y);
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        GameObject ent = (GameObject)Resources.Load(ctype.ToString());
        GameObject m = (GameObject)Object.Instantiate(ent, pos, rot);
        Superior c = m.AddComponent<Superior>();
        c.Init(ctype, health, radius, team, wtype);
        c.Awake();
        return c;
    }
}


public class MinorSpawner : CharacterSpawner
{
    public override void CreateTeam(Team team, TeamStruct teamStruct)
    {
        TeamDesc td=teamStruct.TeamDict[CharType.MinorKnife];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = new Vector2(0, 0);
            spawnPos = TeamFormation.GetBallPosition(0, td.Num, i, spawnPos);
            Minor m = SpawnMinor(team, CharType.MinorKnife, spawnPos, td.Radius, td.WType, td.Health);
            m.FSM.SetCurrentState(MinorIdleState.Instance);
            m.FSM.SetGlobalState(MinorGlobalState.Instance);
            MinorIdleState.Instance.Enter(m);
            MinorGlobalState.Instance.Enter(m);
        }

        td=teamStruct.TeamDict[CharType.MinorSpear];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = new Vector2(0, 0);
            //spawnPos = TeamFormation.GetRingPosition(0, td.Num, i, spawnPos);
            Minor m = SpawnMinor(team, CharType.MinorSpear, spawnPos, td.Radius, td.WType, td.Health);
            m.FSM.SetCurrentState(MinorIdleState.Instance);
            m.FSM.SetGlobalState(MinorGlobalState.Instance);
            MinorIdleState.Instance.Enter(m);
            MinorGlobalState.Instance.Enter(m);
        }
        /*
        td=teamStruct.TeamDict[CharType.MinorMagic];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = GetPosition(2, td.Num, i);
            Minor m = SpawnMinor(team, CharType.MinorMagic, spawnPos, td.Radius, td.WType, td.Health);
            m.FSM.SetCurrentState(MinorIdleState.Instance);
            m.FSM.SetGlobalState(MinorGlobalState.Instance);
            MinorIdleState.Instance.Enter(m);
            MinorGlobalState.Instance.Enter(m);
        }
        */
    }
}

public class SuperiorSpawner : CharacterSpawner
{
    float timer = 0;
    int count = 0;

    public SuperiorSpawner()
    {
        timer = Config.TimeToSpawnEnemy;
    }

    public override void UpdateTeam(Team team, TeamStruct teamStruct)
    {
        TeamDesc td = teamStruct.TeamDict[CharType.Superior];
        if (count<td.Num)
        {
            timer -= 1;
            if (timer <= 0)
            {
                int random = Mathf.FloorToInt(Random.Range(-1, 9));
                if (random == 9 || random == -1)
                    random = 8;
                Vector2 spawnPos = Config.EnterPositions()[random];
                Superior m = SpawnSuperior(team, CharType.Superior, spawnPos, td.Radius, td.WType, td.Health);
                m.FSM.SetCurrentState(SuperiorDefaultIdleState.Instance);
                m.FSM.SetGlobalState(SuperiorDefaultGlobalState.Instance);
                SuperiorDefaultIdleState.Instance.Enter(m);
                SuperiorDefaultGlobalState.Instance.Enter(m);
                ++count;
                timer = Config.TimeToSpawnEnemy;
            }
        }
    }
}
