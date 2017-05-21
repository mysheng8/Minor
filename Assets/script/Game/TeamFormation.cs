using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public enum TeamFormationType
{ 
    Wall,
    Ball,
    Line,
}



public static class TeamFormation 
{

    public static Vector2 GetPointPosition(int index, Vector2 ClickPos)
    {
        return new Vector2(Mathf.Cos(index) * index  + ClickPos.x, Mathf.Sin(index) * index + ClickPos.y);
    }
    public static Vector2 GetBallPosition(float angle, int count, int index, Vector2 ClickPos)
    {
        float radius;
        if (count > 1)
        {
            radius = (float)Config.DistanceEachPoint / Mathf.Sin(2 * Mathf.PI / count);
            if (radius < Config.MinDistanceOfRingTeam)
                radius = Config.MinDistanceOfRingTeam;
        }
        else
        {
            radius = Config.MinDistanceOfRingTeam;
        }
        return new Vector2(radius * Mathf.Cos( 2 * Mathf.PI/ count * index + angle) + ClickPos.x, radius * Mathf.Sin(2 * Mathf.PI / count * index + angle)  + ClickPos.y);
    }

    public static Vector2 GetWallPosition(int line, int index, int count, List<Vector2> HitList, List<Vector2> NormalList) 
    {
        int subIndex = index % HitList.Count;
        int subLine = index / HitList.Count;
        Vector2 pos = HitList[subIndex];
        Vector2 normal = NormalList[subIndex];
        pos -= (subLine+line) * normal * Config.DistanceEachLine ;
        return pos;
    }
}

public class MinorTeamFormat
{
    protected MinorTeamFormat() { }
    public virtual void OnMouseUpdate(TeamStruct ts, List<Character> Members) { }
    protected virtual void UpdateTeam(TeamStruct ts, List<Character> Members) { }
    public virtual Vector2 GetMoveTarget() { return Vector2.zero; }
    public bool CheckGuiRaycastObjects()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.pressPosition = Input.mousePosition;
        eventData.position = Input.mousePosition;

        List<RaycastResult> list = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, list);

        return list.Count > 0;
    }
}

public class LineMinorTeamFormat : MinorTeamFormat
{
    static LineMinorTeamFormat m_Instance;
    List<Character> m_followList;
    
    public static LineMinorTeamFormat Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LineMinorTeamFormat();
            }
            return m_Instance;
        }
    }
    Vector2 hitpos;
    int time = 0;

    LineMinorTeamFormat() 
    { 
        hitpos = Vector2.zero;
        m_followList = new List<Character>();

    }
    public override Vector2 GetMoveTarget() { return hitpos; }
    public override void OnMouseUpdate(TeamStruct ts, List<Character> Members)
    {
        if(CheckGuiRaycastObjects())
            return;
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                hitpos.x = hit.point.x;
                hitpos.y = hit.point.z;
                sortMembers(Members);
            }
        }
        if(Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                hitpos.x = hit.point.x;
                hitpos.y = hit.point.z;
            }
        }
        if (time <= 0)
        {
            sortMembers(Members);
            time = 20;
            
        }
        --time;
        
        UpdateTeam(ts, Members);
    }

    private void sortMembers(List<Character> Members)
    {
        Vector2 target = hitpos;
        m_followList.Clear();
        List<Character> temp = new List<Character>(Members);
        for (int j = 0; j < Members.Count; ++j)
        {
            float minDist = 9999999;
            int closet = -1;
            for (int i = 0; i < temp.Count; ++i)
            {
                float dis = (target - temp[i].Pos).magnitude;
                if (dis < minDist)
                {
                    //Debug.Log(dis + "<" + minDist);
                    minDist = dis;
                    closet = i;
                    
                }
            }
            
            m_followList.Add(temp[closet]);
            target = temp[closet].Pos;
            temp.RemoveAt(closet);
        }
    }

    protected override void UpdateTeam(TeamStruct ts, List<Character> Members)
    {
        
        //Debug.Log(m_followList.Count);
        Vector2 target=hitpos;
        if (m_followList[0].OnGround)
        {
            m_followList[0].Steering.Target = target;
            m_followList[0].FSM.ChangeState(MinorMovingState.Instance);
            target = m_followList[0].Pos;
        }

        for (int i = 1; i < m_followList.Count; ++i)
        {
            if (m_followList[i].OnGround)
            {
                float lookAheadTime = (m_followList[i].Pos - m_followList[i - 1].Pos).magnitude * 4 / (m_followList[i - 1].Movement.MaxSpeed + m_followList[i].Movement.Speed);
                m_followList[i].Steering.Target = target +m_followList[i].Movement.Velocity * lookAheadTime;
                m_followList[i].FSM.ChangeState(MinorMovingState.Instance);
                target = m_followList[i].Pos;
            }
        }
    }
}


public class WallMinorTeamFormat : MinorTeamFormat
{
    static WallMinorTeamFormat m_Instance;
    public static WallMinorTeamFormat Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new WallMinorTeamFormat();
            }
            return m_Instance;
        }
    }
    protected DrawLine m_Drawer;
    protected WallMinorTeamFormat()
    {
        m_Drawer = new DrawLine();
    }

    protected Vector2 m_LineCenter;
    public override Vector2 GetMoveTarget() 
    {
        return m_LineCenter;
    }
   

    public override void OnMouseUpdate(TeamStruct ts, List<Character> Members)
    {
        if (CheckGuiRaycastObjects())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            m_Drawer.DrawBegin();
        }
        if (Input.GetMouseButton(0))
        {
            m_Drawer.Draw();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (m_Drawer.HitList.Count > 1)
            {
                UpdateTeam(ts, Members);

                m_LineCenter = Vector2.zero;
                foreach (Vector2 pos in m_Drawer.HitList)
                {
                    m_LineCenter += pos;
                }
                m_LineCenter /= m_Drawer.HitList.Count;
            }
            m_Drawer.DrawEnd();
        }
    }

    protected override void UpdateTeam(TeamStruct ts, List<Character> Members)
    {
        int buf = 0;
        int linebuf = 1;
        TeamDesc td = ts.TeamDict[CharType.MinorKnife];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = TeamFormation.GetWallPosition(0, i, td.Num, m_Drawer.HitList, m_Drawer.NormalList);
            Members[i].Steering.Target = spawnPos;
            Members[i].FSM.ChangeState(MinorMovingState.Instance);
        }
        buf += td.Num;
        linebuf += td.Num / m_Drawer.HitList.Count;
        Debug.Log(linebuf);
        td = ts.TeamDict[CharType.MinorSpear];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = TeamFormation.GetWallPosition(linebuf, i, td.Num, m_Drawer.HitList, m_Drawer.NormalList);
            Members[buf + i].Steering.Target = spawnPos;
            Members[buf + i].FSM.ChangeState(MinorMovingState.Instance);
        }
        buf += td.Num;
    }
}

public class BallMinorTeamFormat : MinorTeamFormat
{

    static BallMinorTeamFormat m_Instance;
    public static BallMinorTeamFormat Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new BallMinorTeamFormat();
            }
            return m_Instance;
        }
    }
    Vector2 hitpos;
    protected BallMinorTeamFormat() { hitpos = Vector2.zero; }
    public override Vector2 GetMoveTarget() { return hitpos; }
    public override void OnMouseUpdate(TeamStruct ts, List<Character> Members)
    {
        if (!CheckGuiRaycastObjects())
        {
            if (Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    hitpos.x = hit.point.x;
                    hitpos.y = hit.point.z;
                    UpdateTeam(ts,Members);
                }
            }
        }
    }

    protected override void UpdateTeam(TeamStruct ts, List<Character> Members)
    {
        int buf = 0;
        TeamDesc td = ts.TeamDict[CharType.MinorKnife];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = TeamFormation.GetBallPosition(0, td.Num, i, hitpos);
            Members[i].Steering.Target = spawnPos;
            Members[i].FSM.ChangeState(MinorMovingState.Instance);
        }
        buf += td.Num;
        td = ts.TeamDict[CharType.MinorSpear];
        for (int i = 0; i < td.Num; ++i)
        {
            Vector2 spawnPos = TeamFormation.GetPointPosition(i, hitpos);
            Members[buf + i].Steering.Target = spawnPos;
            Members[buf + i].FSM.ChangeState(MinorMovingState.Instance);
        }
        buf += td.Num;
    }
}


