using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : Projectile
{
    int endTimer = Config.TimeToDisappearAfterDead;
    public override void Shoot()
    {
        MaxSpeed = m_WeaponDesc.ShootSpeed;
        //float time = (m_Target.Pos - m_Shooter.Pos).magnitude / m_WeaponDesc.ShootSpeed;
        //Vector2 targetPos = m_Target.Pos + m_Target.Velocity * time;
        //Velocity = (targetPos - m_Shooter.Pos) / time;
        
        Velocity = (m_Target.Pos - m_Shooter.Pos).normalized * m_WeaponDesc.ShootSpeed;

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
        float dis = (m_Target.Pos - Pos).sqrMagnitude;
        if (dis < 2.0f)
        {
            m_Impacted = false;
            m_ImpactPoint = Pos;
            m_End = true;
        }
        dis = (m_Shooter.Pos - Pos).sqrMagnitude;
        if (dis > m_WeaponDesc.MaxHitRange* m_WeaponDesc.MaxHitRange)
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
            Vector2 to = e.Pos - Pos;
            float range = radius + e.BRadius;
            if (e != m_Shooter && to.sqrMagnitude < range * range)
            {
                if (e.IsCharacter())
                {
                    if (m_Shooter.IsEnemy((Character)e))
                    {
                        result = true;
                        ent = e;
                    }
                }
            }
        }
        return result;
    }

    public override void Update()
    {
        base.Update();

        if (m_End)
        {
            endTimer -= 1;
            if (endTimer <= 0)
            {
                World.Projectiles().Remove(this);
                Destroy(gameObject, 0);
            }
        }
        else
        {
            gameObject.transform.position = new Vector3(m_Pos.x, 0, m_Pos.y);
        }
    }

}
