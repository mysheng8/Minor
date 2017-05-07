using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour 
{
    TriggerRegion m_RegionOfInfluence;

    bool m_RemoveFromGame;
    bool m_Active;

    public void SetToBeRemovedFromGame() { m_RemoveFromGame = false; }
    public void SetInactive() { m_Active = false; }
    public void SetActive() { m_Active = true; }
    public bool isTouchingTrigger(Vector2 EntityPos, float EntityRadius)
    {
        if (m_Active)
            return m_RegionOfInfluence.isTouching(EntityPos, EntityRadius);
        else
            return false;
    }
    public bool isBeRemoved() { return m_RemoveFromGame; }
    public bool isActive() { return m_Active; }

    public virtual void Try(Character entity){}
        

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class Respawning_Trigger: Trigger
{
    int m_NumUpdateBetweenRespawns;
    int m_RemainingNumUpdatesRespawn;

    void Deactive()
    {
        SetInactive();
        m_RemainingNumUpdatesRespawn = m_NumUpdateBetweenRespawns;
    }

    void Update()
    {
        if ((--m_RemainingNumUpdatesRespawn) <= 0 && !isActive())
        {
            SetActive();
        }
    }
}

public class LimitedLifetime_Trigger : Trigger
{
    int m_Lifetime;
    void Update()
    {
        if (--m_Lifetime <= 0)
        {
            SetToBeRemovedFromGame();
        }
    }
}

public class Interval_Trigger : Trigger
{
    int m_Lifetime;
    int m_RemainingLifetime;
    int m_NumUpdateBetweenRespawns;
    int m_RemainingNumUpdatesUntilRespawn;
    void Update()
    {
        if (isActive())
        {
            if (--m_Lifetime <= 0)
            {
                SetInactive();
                m_RemainingNumUpdatesUntilRespawn = m_NumUpdateBetweenRespawns;
            }
        }
        else
        {
            if ((--m_RemainingNumUpdatesUntilRespawn) <= 0)
            {
                SetActive();
            }
        }
    }
}
