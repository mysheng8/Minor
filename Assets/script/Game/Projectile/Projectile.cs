using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile : MovingEntity
{
    public Character m_Shooter;
    public Character m_Target;
    public WeaponDesc m_WeaponDesc;
    protected bool m_Impacted = false;
    protected Vector2 m_ImpactPoint;
    
    protected bool m_End;

    public void Init(Character shooter, Character target, WeaponDesc weaponDesc)
    {
        m_Shooter = shooter;
        m_Target = target;
        m_WeaponDesc = weaponDesc;
        Shoot();
    }

    public virtual void Shoot() { }
    public virtual void Hit() { }

	// Use this for initialization
	public void Awake () {
        base.Awake();
	}
	
	// Update is called once per frame
    public virtual void Update()
    {

        if (!m_End)
        {
            Hit();
            base.Update();
        }

	}
}


public class ProjectileExtraInfo
{
    public int Damage;
    public Character Shooter;
    public float BackForward;
    public ProjectileExtraInfo(int damage, float backforward, Character shooter)
    {
        Damage = damage;
        Shooter = shooter;
        BackForward = backforward;
    }
}

