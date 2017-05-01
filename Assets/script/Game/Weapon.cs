using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum WeaponType
{ 
    Wpn_Spear,
    Wpn_Knife,
    Wpn_Magic,
}

public class WeaponDesc
{
    public float ShootRange;
    public float RateOfFire;
    public int Damage;
    public float ShootSpeed;
    public float ImpactRange;
    public string ProjectileType;
    public float BackForward;
    public float MaxHitRange;
    public WeaponDesc(float shootRange, float rateOfFire, int damage, float shootSpeed, float impactRange, float backForward, float maxHitRange, string projectileType)
    {
        ShootRange = shootRange;
        RateOfFire = rateOfFire;
        Damage = damage;
        ShootSpeed = shootSpeed;
        ImpactRange = impactRange;
        BackForward = backForward;
        ProjectileType = projectileType;
        MaxHitRange = maxHitRange;
    }
}

public class Weapon {

    WeaponType  m_WeaponType;
    WeaponDesc m_WeaponDesc;
    Character m_Owner;

    float timer = 0;

    public Weapon(WeaponType type, Character owner)
    {
        m_WeaponType = type;
        m_WeaponDesc = Config.WeaponDict()[type];
        m_Owner = owner;
    }

    public bool FindEnemy(List<Character> enemies)
    {
        bool result = false;
        float MaxDist = Config.MaxAlertDistance;
        foreach (Character ent in enemies)
        {
            Vector2 distanceToEnemy = ent.Pos - m_Owner.Pos;
            if (distanceToEnemy.sqrMagnitude < MaxDist * MaxDist)
            {
                MaxDist = distanceToEnemy.magnitude;
                m_Owner.Target = ent;
                result = true;
            }
        }
        return result;
    }

    public bool AimAt(Vector2 target, bool move)
    {
        Vector2 Dir = target - m_Owner.Pos;
        Vector3 realtivePos = new Vector3(Dir.x, 0, Dir.y);
        m_Owner.transform.rotation = Quaternion.LookRotation(realtivePos);
        if (Dir.magnitude <= m_WeaponDesc.ShootRange)
        {
            if (move)
                m_Owner.Steering.IsArrive = false;
            return true;
        }
        else
        {
            if (move)
            {
                Vector2 tar = Vector2.ClampMagnitude(Dir, Dir.magnitude - m_WeaponDesc.ShootRange + m_Owner.BRadius) + m_Owner.Pos;
                m_Owner.Steering.Target = tar;
                m_Owner.Steering.IsArrive = true;
            }
            return false;
        }

    }

    public void ShootAt(Character enemy)
    {
        /*
        Vector3 p1 = new Vector3(m_Owner.Pos.x, 0, m_Owner.Pos.y);
        Vector3 t1 = new Vector3(target.x, 0, target.y);
        Debug.DrawLine(p1, t1);*/
        timer -= 1;
        
        if (timer <= 0)
        {
            GameObject ent = Resources.Load(Config.WeaponDict()[m_WeaponType].ProjectileType) as GameObject;
            
            
            float time = (enemy.Pos - m_Owner.Pos).magnitude / m_WeaponDesc.ShootSpeed;
            //Vector2 expectPos = enemy.Pos + enemy.MaxSpeed * enemy.Velocity * time;
            Vector2 Dir = enemy.Pos - m_Owner.Pos;
            Vector3 realtivePos = new Vector3(Dir.x, 0, Dir.y);
            Vector3 pos = new Vector3(m_Owner.Pos.x, 0, m_Owner.Pos.y);
            Quaternion rot = Quaternion.LookRotation(realtivePos);

            GameObject pObject = (GameObject)UnityEngine.Object.Instantiate(ent, pos, rot);
            
            Type t = Type.GetType(Config.WeaponDict()[m_WeaponType].ProjectileType);
            Projectile p = (Projectile)pObject.AddComponent(t);
            p.Init(m_Owner, enemy, m_WeaponDesc);
            m_Owner.World.Projectiles().Add(p);
            timer = m_WeaponDesc.RateOfFire;
        }
    }

    

    public float GetShootSpeed()
    {
        return m_WeaponDesc.ShootSpeed;
    }

    public WeaponType GetWeaponType()
    {
        return m_WeaponType;
    }
}
