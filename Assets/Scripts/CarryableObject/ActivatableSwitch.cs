using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActivatableSwitch : MonoBehaviour
{
    [SerializeField] Vector3 m_captureSize = Vector3.one;

    [SerializeField] UnityEvent m_onSwitchActivate;
    [SerializeField] UnityEvent m_onSwitchDeactivate;

    [SerializeField] LayerMask m_captureLayer = 0;

    bool m_isActivated = false;

    [Header("Animation")]
    [SerializeField] Transform m_buttonTransform;
    [SerializeField] float m_buttonSpeed = 1.0f;
    float m_animationTimer = 1.0f;
    float m_animationDirection = 1.0f;

    Vector3 m_buttonOrigin = Vector3.zero;

    private void Awake()
    {
        m_buttonOrigin = m_buttonTransform.position;
    }

    private void Update()
    {
        bool wasActivated = m_isActivated;
        m_isActivated = Physics.CheckBox(GetCaptureCentre(), m_captureSize * 0.5f, transform.rotation, m_captureLayer, QueryTriggerInteraction.Ignore);

        if(wasActivated != m_isActivated)
        {
            // Change detected
            if(m_isActivated)
            {
                m_onSwitchActivate.Invoke();
                m_animationDirection = -1.0f;
            }
            else
            {
                m_onSwitchDeactivate.Invoke();
                m_animationDirection = 1.0f;
            }
        }

        m_animationTimer += Time.deltaTime * m_buttonSpeed * m_animationDirection;
        m_animationTimer = Mathf.Clamp(m_animationTimer, 0.0f, 1.0f);
        m_buttonTransform.position = Vector3.Lerp(transform.position, m_buttonOrigin, m_animationTimer);
    }

    Vector3 GetCaptureCentre()
    {
        return transform.position + transform.up * m_captureSize.y * 0.5f;
    }
}
