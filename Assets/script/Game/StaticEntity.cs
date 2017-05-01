using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEntity : BaseEntity {

    public override bool IsStaticEntity() { return true; }
	// Use this for initialization
	public void Awake () {
        base.Awake();
        m_Pos = new Vector2(transform.position.x, transform.position.z);
    }

    // Update is called once per frame
    public void Update () {
        base.Update();
	}

    public void RollingMap(float speed, Vector2 direction)
    {
        m_Pos += direction * speed * Time.deltaTime;
        transform.position = new Vector3(m_Pos.x, 0, m_Pos.y);
    }
}
