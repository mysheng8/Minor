using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Superior : Character
{
    // Use this for initialization
    protected void Awake()
    {
        base.Awake();
        Health = Config.SuperiorHealth;
        Weapon = new Weapon(WeaponType.Wpn_Knife, this);
    }

    // Update is called once per frame
    protected void Update()
    {
        base.Update();
    }
}
