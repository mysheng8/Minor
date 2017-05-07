using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : Projectile
{
    int endTimer = Config.TimeToKillByKnife;
    public override void Shoot()
    {
        //float time = (m_Target.Pos - m_Shooter.Pos).magnitude / m_Speed;
        //Vector2 targetPos = m_Target.Pos + m_Target.Velocity * time;
        //Velocity = (targetPos - m_Shooter.Pos) / time;

        

    }
    public override void Hit()
    {
        World.Partition.CalculateNeighbors(Pos);
        BaseEntity ent;
        if (HitTest(World.Partition.Neighbors(), m_WeaponDesc.ImpactRange, out ent))
        {
            //MessageDispatcher.Instance.DispatchMessage(0, this, ent, MessageType.Msg_Damage, new ProjectileExtraInfo(m_Damage, 0, m_Shooter));
            MessageDispatcher.Instance.DispatchMessage(0, this, ent, MessageType.Msg_Push, new ProjectileExtraInfo(m_WeaponDesc.Damage, m_WeaponDesc.BackForward, m_Shooter));
            m_Impacted = true;
            m_ImpactPoint = Pos;
        }
        endTimer -= 1;
        if (endTimer <= 0)
        {
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
                result = true;
                ent = e;
            }
        }
        return result;
    }
    public override void Update()
    {
        base.Update();

        if (m_End)
        {
            World.Projectiles().Remove(this);
            Destroy(gameObject, 0);
        }
        else
        {
            float step = 1000 * Time.deltaTime;
            Vector3 target = gameObject.transform.rotation.eulerAngles;
            target.y -= 270;
            gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, Quaternion.Euler(target), step);
        }
    }
}
