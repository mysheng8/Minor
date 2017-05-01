using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T> 
{
    T m_Owner;
    State<T> m_CurrentState;
    State<T> m_PreviousState;
    State<T> m_GlobalState;

    public StateMachine(T owner)
    {
        m_Owner=owner;
    }

    public State<T> CurrentState()
    {
        return m_CurrentState;
    }

    public State<T> GlobalState()
    {
        return m_GlobalState;
    }

    public State<T> PreviousState()
    {
        return m_PreviousState;
    }

    public void SetCurrentState(State<T> s)
    {
        m_CurrentState = s;
    }

    public void SetGlobalState(State<T> s)
    {
        m_GlobalState = s;
    }

    public void SetPreviousState(State<T> s)
    {
        m_PreviousState = s;
    }
	
	// Update is called once per frame
	public void Update () {
        if (m_GlobalState != null)
            m_GlobalState.Execute(m_Owner);
        if (m_CurrentState != null)
            m_CurrentState.Execute(m_Owner);
	}

    public void ChangeState(State<T> NewState)
    {
        if (NewState == null)
            Debug.LogAssertion("trying to change to a null state");
        m_PreviousState = m_CurrentState;
        m_CurrentState.Exit(m_Owner);
        m_CurrentState = NewState;
        m_CurrentState.Enter(m_Owner);
     }

    public void RevertToPreviousState()
    {
        ChangeState(m_PreviousState);
    }

    public bool IsInState(State<T> s)
    {
        return s == m_CurrentState;
    }

    public bool HandleMessage(Telegram msg)
    {
        if (m_CurrentState != null && m_CurrentState.OnMessage(m_Owner, msg))
        {
            return true;
        }
        if (m_GlobalState != null && m_GlobalState.OnMessage(m_Owner, msg))
        {
            return true;
        }
        return false;
    }
}
