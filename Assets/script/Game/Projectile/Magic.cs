using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : Projectile
{
    public override void Shoot()
    {
        //float time = (m_Target.Pos - m_Shooter.Pos).magnitude / m_Speed;
        //Vector2 targetPos = m_Target.Pos + m_Target.Velocity * time;
        //Velocity = (targetPos - m_Shooter.Pos) / time;
        Velocity = m_Target.Pos - m_Shooter.Pos;
    }

}
