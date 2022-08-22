using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterAnimate
{
    [SerializeField] Animator m_animator;

    [SerializeField] string m_movementParameter = "Movement";

    [SerializeField] string[] m_stateNames = { "MotionTree", "Climb_Up_Wall", "Falling_Idle" };

    public void CrossFade(int index, float normalisedTransitionDuration)
    {
        m_animator.CrossFade(m_stateNames[index], normalisedTransitionDuration);
    }

    public void CrossFade(string stateName, float normalisedTransitionDuration)
    {
        m_animator.CrossFade(stateName, normalisedTransitionDuration);
    }

    public void SetMovementParameter(float value)
    {
        m_animator.SetFloat(m_movementParameter, value);
    }
}
