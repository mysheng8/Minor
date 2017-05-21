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
        base.Update();
        
    }
}
