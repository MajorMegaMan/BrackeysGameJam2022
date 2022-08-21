using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    T m_owner = default;
    IState<T> m_currentState = null;

    public StateMachine(T owner)
    {
        m_owner = owner;
    }

    public void InitialiseState(IState<T> initialState)
    {
        m_currentState = initialState;
        m_currentState.Enter(m_owner);
    }

    public void SetState(IState<T> nextState)
    {
        m_currentState.Exit(m_owner);
        m_currentState = nextState;
        m_currentState.Enter(m_owner);
    }

    public void Invoke()
    {
        m_currentState.Invoke(m_owner);
    }
}

public interface IState<T>
{
    void Enter(T owner);
    void Exit(T owner);
    void Invoke(T owner);
}
