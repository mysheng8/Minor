﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Projectile
{
    int endTimer = Config.TimeToDisappearAfterDead;
    public override void Shoot()
    {
        m_Movement = new ForwardMovement(m_Target.GetWorldPosition(), m_Shooter.GetWorldPosition(), m_WeaponDesc.ShootSpeed);
    }

    public override void Hit()
    {
        World.Partition.CalculateNeighbors(Pos);
        BaseEntity ent;
        if (HitTest(World.Partition.Neighbors(), m_WeaponDesc.ImpactRange, out ent))
        {
            MessageDispatcher.Instance.DispatchMessage(0, this, ent, MessageType.Msg_Damage, new ProjectileExtraInfo(m_WeaponDesc.Damage, m_WeaponDesc.BackForward, m_Shooter));
            m_Impacted = true;
            m_ImpactPoint = Pos;
            m_End = true;
            gameObject.transform.parent = ent.gameObject.transform;
        }

        if (((ForwardMovement)Movement).IsArrived())
        {
            m_Impacted = false;
            m_ImpactPoint = Pos;
            m_End = true;
        }

        float ground_height = World.GetHeight(Pos);
        if (Pos.y <= ground_height)
        {
            m_Impacted = false;
            m_ImpactPoint = Pos;
            m_End = true;       
        }


    }

    bool HitTest(List<BaseEntity> ContainerOfEntities, float radius, out BaseEntity ent)
    {
        bool result = false;
        ent = null;
        foreach (BaseEntity e in ContainerOfEntities)
        {
            if (e.HitTest(Pos, BRadius) && e != m_Shooter)
            {
                float deltaHeight = Height - e.Height;
                if (deltaHeight < BRadius + e.BRadius)
                {
                    result = true;
                    ent = e;
                }
            }
        }
        return result;
    }

    public override void Update()
    {
        base.Update();
        //keep object a while after it impacted.
        if (m_End)
        {
            endTimer -= 1;
            if (endTimer <= 0)
            {
                World.Projectiles().Remove(this);
                Destroy(gameObject, 0);
            }
        }
    }

}
