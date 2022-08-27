using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnim : MonoBehaviour
{
    [SerializeField] Animator m_animator;

    [SerializeField] string m_stateName = "TargetState";

    // Start is called before the first frame update
    void Start()
    {
        m_animator.CrossFade(m_stateName, 0.1f);
    }
}
