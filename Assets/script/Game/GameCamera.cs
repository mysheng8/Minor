using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum CameraBehaviourType
{
    Wall,
    Ball,
    Line,
}

public class GameCamera : MonoBehaviour {

    GameCameraBehaviour m_CameraBehaviour;
    MinorTeam m_Team;
    Vector3 m_CamPos;
    Vector3 m_TargetPos;
    public Vector3 GetCameraPos()
    {
        return m_CamPos;
    }

    public Vector3 GetTargetPos()
    {
        return m_TargetPos;
    }

    public void SetCameraBehaviourType(CameraBehaviourType tType)
    {
        switch (tType)
        {
            default:
                m_CameraBehaviour = LineGameCameraBehaviour.Instance;
                break;
            case CameraBehaviourType.Line:
                m_CameraBehaviour = LineGameCameraBehaviour.Instance;
                break;
            case CameraBehaviourType.Wall:
                m_CameraBehaviour = WallGameCameraBehaviour.Instance;
                break;
            case CameraBehaviourType.Ball:
                m_CameraBehaviour = BallGameCameraBehaviour.Instance;
                break;
        }
    }

    // Use this for initialization
    void Awake () {
        m_CamPos = Vector3.zero;
        m_TargetPos = Vector3.zero;
        m_CameraBehaviour = LineGameCameraBehaviour.Instance;
        m_Team = GameWorld.Instance.GetTeam();
    }
	
	// Update is called once per frame
	void Update () {
        m_CameraBehaviour.RolingMap(m_Team, ref m_TargetPos, ref m_CamPos);
        gameObject.transform.position = m_CamPos;

    }
}


public class GameCameraBehaviour
{
    public float m_RollingSpeed = 0;
    public Vector3 m_RollingDir = Vector3.zero;
    public virtual void RolingMap(MinorTeam team, ref Vector3 tarPos, ref Vector3 camPos) {}
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

public class LineGameCameraBehaviour : GameCameraBehaviour
{
    static LineGameCameraBehaviour m_Instance;
    public static LineGameCameraBehaviour Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LineGameCameraBehaviour();
            }
            return m_Instance;
        }
    }

    public override void RolingMap(MinorTeam team, ref Vector3 tarPos, ref Vector3 camPos)  
    {
        if (Input.GetMouseButton(0))
        {
            if(!CheckGuiRaycastObjects())
            {
                Vector2 hit = team.GetMoveTarget();
                float height = GameWorld.Instance.GetHeight(hit);
                tarPos = new Vector3(hit.x, height, hit.y);
            }
        }
        m_RollingDir = (tarPos - camPos).normalized;
        m_RollingSpeed = (tarPos - camPos).magnitude / 1.5f;
        if (m_RollingSpeed > Config.gCameraMovementMaxSpeed)
            m_RollingSpeed = Config.gCameraMovementMaxSpeed;
        if (GameWorld.Instance.CurrentLevel.IsBehindMap(camPos - new Vector3(Screen.width / 2, 0, 0)))
        {
            if (m_RollingDir.x < 0)
            {
                m_RollingDir.x = 0;
                m_RollingDir=m_RollingDir.normalized;
            }
        }
        camPos += m_RollingDir * m_RollingSpeed * Time.deltaTime;

    }
}

public class WallGameCameraBehaviour : GameCameraBehaviour
{
    static WallGameCameraBehaviour m_Instance;
    public static WallGameCameraBehaviour Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new WallGameCameraBehaviour();
            }
            return m_Instance;
        }
    }

    public override void RolingMap(MinorTeam team, ref Vector3 tarPos, ref Vector3 camPos)  
    {
        if (Input.GetMouseButton(0))
        {
            if (!CheckGuiRaycastObjects())
            {
                Vector2 hit = team.GetMoveTarget();
                float height = GameWorld.Instance.GetHeight(hit);
                tarPos = new Vector3(hit.x, height, hit.y);
            }
        }
        m_RollingDir = (tarPos - camPos).normalized;
        m_RollingSpeed = (tarPos - camPos).magnitude / 1.5f;
        if (m_RollingSpeed > Config.gCameraMovementMaxSpeed)
            m_RollingSpeed = Config.gCameraMovementMaxSpeed;
        if (GameWorld.Instance.CurrentLevel.IsBehindMap(camPos - new Vector3(Screen.width / 2, 0, 0)))
        {
            if (m_RollingDir.x < 0)
            {
                m_RollingDir.x = 0;
                m_RollingDir = m_RollingDir.normalized;
            }
        }
        camPos += m_RollingDir * m_RollingSpeed * Time.deltaTime;
    }
}

public class BallGameCameraBehaviour : GameCameraBehaviour
{
    static BallGameCameraBehaviour m_Instance;
    public static BallGameCameraBehaviour Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new BallGameCameraBehaviour();
            }
            return m_Instance;
        }
    }

    public override void RolingMap(MinorTeam team,ref  Vector3 tarPos, ref Vector3 camPos)  
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!CheckGuiRaycastObjects())
            {
                Vector2 hit = team.GetMoveTarget();
                float height = GameWorld.Instance.GetHeight(hit);
                tarPos = new Vector3(hit.x, height, hit.y);
            }
        }
        m_RollingDir = (tarPos - camPos).normalized;
        m_RollingSpeed = (tarPos - camPos).magnitude / 1.5f;
        if (m_RollingSpeed > Config.gCameraMovementMaxSpeed)
            m_RollingSpeed = Config.gCameraMovementMaxSpeed;
        if (GameWorld.Instance.CurrentLevel.IsBehindMap(camPos - new Vector3(Screen.width / 2, 0, 0)))
        {
            if (m_RollingDir.x < 0)
            {
                m_RollingDir.x = 0;
                m_RollingDir = m_RollingDir.normalized;
            }
        }
        camPos += m_RollingDir * m_RollingSpeed * Time.deltaTime;
    }
}