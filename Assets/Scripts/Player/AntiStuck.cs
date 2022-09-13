using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AntiStuck : MonoBehaviour
{
    [SerializeField] float m_stuckTime = 2.0f;
    [SerializeField] UnityEvent m_unstuckEvent;

    float m_timer = 0.0f;

    public void ResetTimer()
    {
        m_timer = 0.0f;
    }

    private void OnCollisionStay(Collision collision)
    {
        m_timer += Time.deltaTime;
        if(m_timer > m_stuckTime)
        {
            m_unstuckEvent.Invoke();
        }
    }
}
