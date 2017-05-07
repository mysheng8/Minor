using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minor : Character 
{
	// Use this for initialization
    protected void Awake()
    {
        base.Awake();
        Health = Config.MinorHealth;
        IsJumpable = true;
	}
	
	// Update is called once per frame
    protected void Update()
    {
        EnforceNonOutWallConstraint(this, World.Walls());
        
        base.Update();
    }

    void EnforceNonOutWallConstraint(Character m, List<Wall> walls)
    {
        foreach (Wall w in walls)
        {
            Vector2 dir=m.Pos-w.From();
            float dis = yMath.DistPointToLine2D(m.Pos, w.From(), w.To());
            if (Vector2.Dot(w.Normal(), dir) > 0)
            {
                float AmountOfOverLap = dis - m.BRadius;
                if (AmountOfOverLap <= Config.WallThickness && AmountOfOverLap>0)
                {
                    Vector2 move = w.Normal() * (Config.WallThickness - AmountOfOverLap);
                    m.Pos += move;
                }
            }
        }
    }


}
