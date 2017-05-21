using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config {


    public static float gCameraMovementMaxSpeed = 140;
    
    public static float Gravity = -10;
    public static float WallThickness = 5;
    public static float MinDetectionWallDistance = 100;
    public static float MinDetectionJumpDistance = 20;
    public static float MinDetectionBoxLength = 15;
    public static float NumSecondUpdateEntityPosition = 2;
    public static float WallMaxDistance = 99999;
    public static float ObstacleMaxDistance = 10;
    public static float CrowdlMaxDistance = 10;
    public static float MaxSpeedWander = 25;
    public static float MaxSpeedJump = 50;
    public static float MaxSpeedAttention = 40;
    public static float MaxSpeedMoving = 100;
    public static float MaxSpeedFlee = 150;
    public static float MaxAlertDistance = 50;

    public static float TelegramEqualDelteTime = 0.3f;


    public static int DefaultHealth = 100;
    public static int MinorHealth = 100;
    public static int SuperiorHealth = 300;

    public static int MinorSpearHealth = 80;
    public static int MinorKnifeHealth = 200;
    public static int MinorMagicHealth = 60;

    public static int DefaultRadius = 1;
    public static int MinorRadius = 2;
    public static int SuperiorRadius = 4;
    //
    public static int TimeToSpawnEnemy = 100;
    public static List<Vector2> EnterPositions()
    {
        List<Vector2> posList = new List<Vector2>();
        posList.Add(new Vector2(150, 100));
        posList.Add(new Vector2(150, 0));
        posList.Add(new Vector2(150, -100));
        posList.Add(new Vector2(100,150));
        posList.Add(new Vector2(0, 150));
        posList.Add(new Vector2(-100, 150));
        posList.Add(new Vector2(-150, 100));
        posList.Add(new Vector2(-150, 0));
        posList.Add(new Vector2(-150, -100));
        //posList.Add(new Vector2(100, -150));
        //posList.Add(new Vector2(0, -150));
        //posList.Add(new Vector2(-100, -150));
        return posList;
    }

    public static List<Vector2> ExitPositions()
    {
        List<Vector2> posList = new List<Vector2>();
        posList.Add(new Vector2(100, -200));
        posList.Add(new Vector2(0, -200));
        posList.Add(new Vector2(-100, -200));
        posList.Add(new Vector2(100, 200));
        posList.Add(new Vector2(0, 200));
        posList.Add(new Vector2(-100, 200));
        posList.Add(new Vector2(200, 100));
        posList.Add(new Vector2(200, 0));
        posList.Add(new Vector2(200, -100));
        posList.Add(new Vector2(-200, 100));
        posList.Add(new Vector2(-200, 0));
        posList.Add(new Vector2(-200, -100));
        return posList;
    }

    public static List<Vector2> GonePositions()
    {
        List<Vector2> posList = new List<Vector2>();
        posList.Add(new Vector2(100, 200));
        posList.Add(new Vector2(0, 200));
        posList.Add(new Vector2(-100, 200));
        return posList;
    }


    public static int TimeToDisappearAfterDead = 50;
    public static int TimeToKillByKnife = 10;
    public static int TimeToClamDown = 200;
    public static int TimeToStandUpFast = 50;
    public static int TimeToStandUpSlow = 800;
    public static int TimeToStandUpLong = 2000;

    public static int TimeToSampleHit = 10;
    public static int LengthToSampleHit = 15;
    public static int DrawLineWaitTime = 5;

    public static int DistanceEachLine = 15;
    public static int DistanceEachPoint = 15;
    public static int MinDistanceOfRingTeam = 8;
    
    public static int MaxNumberOfMembers = 99;
    //

    public static Dictionary<WeaponType, WeaponDesc> WeaponDict()
    {
        Dictionary<WeaponType, WeaponDesc> dict = new Dictionary<WeaponType, WeaponDesc>();
        dict.Add(WeaponType.Wpn_Spear, new WeaponDesc(60, 50, 35, 120, 1, 0, 50, "Spear"));//1: shootRange, 2: rateOfFire, 3: damage, 4: shootSpeed, 5: impactRange, 6: BackForward, 7: MaxHitRange, 6: projectileType
        dict.Add(WeaponType.Wpn_Knife, new WeaponDesc(10, 15, 15, 0, 10, 1.2f, 0, "Knife"));
        dict.Add(WeaponType.Wpn_Magic, new WeaponDesc(40, 100, 50, 120, 3, 0, 50, "Magic"));
        return dict;
    }

    public static List<string> LevelList()
    {
        List<string> dict = new List<string>();
        dict.Add("level00.xml");
        dict.Add("level01.xml");
        dict.Add("level2way01.xml");
        dict.Add("levelbig01.xml");
        dict.Add("leveldown01.xml");
        dict.Add("levelup01.xml");
        return dict;
    }

    
}


public class HeightmapConfig
{
    public static float heightmapSampleUnitSizeX = 2.0f;
    public static float heightmapSampleUnitSizeY = 2.0f;
    public static float heightmapMaxHeightDistance = 100.0f;
    public static int heightmapTextureSizeX = 512;
    public static int heightmapTextureSizeY = 512;
}
