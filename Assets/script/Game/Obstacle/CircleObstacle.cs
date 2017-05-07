using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleObstacle : Obstacle
{

    public override bool HitTest(Vector2 entityPos, float entityRadius)
    {
        return yMath.CircleHitTest(Pos, BRadius, entityPos, entityRadius);
    }

    public override Vector2 CalculatePenetrationConstraint(Vector2 entityPos, float entityRadius)
    {
        return yMath.CalculateCircleOverlay(Pos, StopDistance+BRadius, entityPos, entityRadius);
    }
    // Use this for initialization
    public void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
