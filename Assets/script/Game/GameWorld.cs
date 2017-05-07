using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameWorld : MonoBehaviour {
    public bool isEditorMode;
    private static GameWorld m_Instance;
    


    GameLevel m_previousLevel;
    GameLevel m_currentLevel;
    GameLevel m_nextLevel;
    GameCamera m_camera;

    public List<Projectile> m_Projectiles;
    private MinorTeam m_teamA;
    private Team m_teamB;

    public MinorTeam GetTeam()
    {
        return m_teamA;
    }

    public Team GetOpponent(Team t)
    {
        if (t == m_teamA)
            return m_teamB;
        else
            return m_teamA;
    }

    public GameLevel CurrentLevel
    {
        get
        {
            return m_currentLevel;
        }
    }

    public void OnLineButtonClick()
    {
        m_teamA.SetMTF(TeamFormationType.Line);
        m_camera.SetCameraBehaviourType(CameraBehaviourType.Line);
        Debug.Log("Line Team");
    }

    public void OnBallButtonClick()
    {
        m_teamA.SetMTF(TeamFormationType.Ball);
        m_camera.SetCameraBehaviourType(CameraBehaviourType.Ball);
        Debug.Log("Ball Team");
    }

    public void OnWallButtonClick()
    {
        m_teamA.SetMTF(TeamFormationType.Wall);
        m_camera.SetCameraBehaviourType(CameraBehaviourType.Wall);
        Debug.Log("Wall Team");
    }

    void BindUI()
    {
        GameObject linebtnObj = GameObject.Find("LineButton");
        Button linebtn = linebtnObj.GetComponent<Button>();
        linebtn.onClick.AddListener(OnLineButtonClick);

        GameObject wallbtnObj = GameObject.Find("WallButton");
        Button wallbtn = wallbtnObj.GetComponent<Button>();
        wallbtn.onClick.AddListener(OnWallButtonClick);

        GameObject ballbtnObj = GameObject.Find("BallButton");
        Button ballbtn = ballbtnObj.GetComponent<Button>();
        ballbtn.onClick.AddListener(OnBallButtonClick);
    }

    GameLevel spawnNextLevel(bool isFirst)
    {
        GameLevel next = new GameLevel();
        if (isFirst)
        {
            next.Init(new Vector2(-60,0), Config.LevelList()[0]);
        }
        else
        {
            int n = Config.LevelList().Count;
            int i = (int)(Random.value*n);
            next.Init(m_currentLevel.ExitPos(), Config.LevelList()[0]);
        }
        return next;   
    }

    // Use this for initialization
    void Awake () {
        if (!isEditorMode)
        {
            m_currentLevel = spawnNextLevel(true);
            m_previousLevel = m_currentLevel;
            m_nextLevel = spawnNextLevel(false);
            m_camera = GameObject.Find("CameraRoot").GetComponent<GameCamera>();
            m_camera.SetCameraBehaviourType(CameraBehaviourType.Line);
            TeamStruct mts = new TeamStruct();
            mts.TeamDict.Add(CharType.MinorKnife, new TeamDesc(Config.MinorKnifeHealth, 4, 15, WeaponType.Wpn_Knife));
            mts.TeamDict.Add(CharType.MinorSpear, new TeamDesc(Config.MinorSpearHealth, 4, 8, WeaponType.Wpn_Spear));
            //mts.TeamDict.Add(CharType.MinorMagic, new TeamDesc(Config.MinorMagicHealth, 2, 5, WeaponType.Wpn_Magic));


            m_teamA = new MinorTeam(mts, 23);
            m_teamA.Spawner = new MinorSpawner();
            m_teamA.OnStart();

            TeamStruct sts = new TeamStruct();
            sts.TeamDict.Add(CharType.Superior, new TeamDesc(Config.SuperiorHealth, 4, 0, WeaponType.Wpn_Knife));

            m_teamB = new SuperiorTeam(sts, 0);
            m_teamB.Spawner = new SuperiorSpawner();
            m_teamB.OnStart();

            BindUI();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (!isEditorMode)
        {
            m_teamA.OnUpdate();
            m_teamB.OnUpdate();
            OutOfMap();
        }
	}
    public void OutOfMap()
    {

        if (m_currentLevel.IsExceedCellSpace(m_camera.GetTargetPos()))
        {
            m_currentLevel = m_nextLevel;
            Vector2 outPos = Vector2.zero;
            foreach (Character c in m_teamA.Members())
            {
                if (m_currentLevel.IsBehindCellSpace(c.Pos, out outPos))
                    c.Pos = outPos;
                m_currentLevel.Partition().AddEntity(c);
            }

        }
        Vector2 cameraborder = m_camera.GetCameraPos() - new Vector2(Screen.width/2, 0);
        if (m_previousLevel.IsExceedCellSpace(cameraborder))
        {
            m_previousLevel.RemoveAll();
            m_nextLevel = spawnNextLevel(false);
            m_previousLevel = m_currentLevel;
        }

    }

    public float GetHeight(Vector2 pos)
    {
        if (m_currentLevel.IsExceedMap(pos))
        {
            return m_nextLevel.GetHeight(pos);
        }
        else if (m_currentLevel.IsBehindMap(pos))
        {
            return m_previousLevel.GetHeight(pos);
        }
        else 
        {
            return m_currentLevel.GetHeight(pos);
        }
    }

    public List<Wall> Walls()
    {
        return m_currentLevel.Walls();
    }

    public List<Obstacle> Obstacles()
    {
        return m_currentLevel.Obstacles();
    }

    public List<Projectile> Projectiles()
    {
        return m_Projectiles;
    }

    public CellSpacePartition Partition
    {
        get
        {
            return m_currentLevel.Partition();
        }
    }

    public static GameWorld Instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject root = GameObject.Find("root");
                m_Instance = root.GetComponent<GameWorld>();
            }

            return m_Instance;
        }
    }

    public void OnApplicationQuit()
    {
        m_Instance = null;
    }
}
