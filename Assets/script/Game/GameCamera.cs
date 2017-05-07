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
    MinorTeam m_team;

    Vector2 m_CamPos;
    Vector2 m_TargetPos;
    public Vector2 GetCameraPos()
    {
        return m_CamPos;
    }

    public Vector2 GetTargetPos()
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
        m_CamPos = Vector2.zero;
        m_TargetPos = Vector2.zero;
        m_CameraBehaviour = LineGameCameraBehaviour.Instance;
        m_team=GameWorld.Instance.GetTeam();
	}
	
	// Update is called once per frame
	void Update () {
        m_CameraBehaviour.RolingMap(m_team, ref m_TargetPos, ref m_CamPos);
        gameObject.transform.position = new Vector3(m_CamPos.x, 0, m_CamPos.y);

    }
}


public class GameCameraBehaviour
{
    public float m_RollingSpeed = 0;
    public Vector2 m_RollingDir = Vector2.zero;
    public virtual void RolingMap(MinorTeam team, ref Vector2 tarPos, ref Vector2 camPos) {}
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

    public override void RolingMap(MinorTeam team, ref Vector2 tarPos, ref Vector2 camPos)  
    {
        if (Input.GetMouseButton(0))
        {
            if(!CheckGuiRaycastObjects())
            {
                tarPos = team.GetMoveTarget();
            }
        }
        m_RollingDir = (tarPos - camPos).normalized;
        m_RollingSpeed = (tarPos - camPos).magnitude / 1.5f;
        if (m_RollingSpeed > Config.gCameraMovementMaxSpeed)
            m_RollingSpeed = Config.gCameraMovementMaxSpeed;
        camPos += new Vector2(m_RollingDir.x * m_RollingSpeed * Time.deltaTime, m_RollingDir.y * m_RollingSpeed * Time.deltaTime);
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

    public override void RolingMap(MinorTeam team, ref Vector2 tarPos, ref Vector2 camPos)  
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!CheckGuiRaycastObjects())
            {
                tarPos = team.GetMoveTarget();
            }
        }
        m_RollingDir = (tarPos - camPos).normalized;
        m_RollingSpeed = (tarPos - camPos).magnitude / 1.5f;
        if (m_RollingSpeed > Config.gCameraMovementMaxSpeed)
            m_RollingSpeed = Config.gCameraMovementMaxSpeed;
        camPos += new Vector2(m_RollingDir.x * m_RollingSpeed * Time.deltaTime, m_RollingDir.y * m_RollingSpeed * Time.deltaTime);
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

    public override void RolingMap(MinorTeam team,ref  Vector2 tarPos, ref Vector2 camPos)  
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!CheckGuiRaycastObjects())
            {
                tarPos = team.GetMoveTarget();
            }
        }
        m_RollingDir = (tarPos - camPos).normalized;
        m_RollingSpeed = (tarPos - camPos).magnitude / 1.5f;
        if (m_RollingSpeed > Config.gCameraMovementMaxSpeed)
            m_RollingSpeed = Config.gCameraMovementMaxSpeed;
        camPos += new Vector2(m_RollingDir.x * m_RollingSpeed * Time.deltaTime, m_RollingDir.y * m_RollingSpeed * Time.deltaTime);
    }
}