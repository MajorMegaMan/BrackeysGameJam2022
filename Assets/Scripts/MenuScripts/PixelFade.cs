using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PixelFade : MonoBehaviour
{
    [SerializeField] ExtendedPixelController m_pixelController = null;
    [SerializeField] float m_fadeInTime = 1.0f;
    [SerializeField] float m_fadeOutTime = 1.0f;
    [SerializeField] float m_beginPixelRatio = 200.0f;

    delegate void UpdateDelegate();
    UpdateDelegate m_updateDelegate;

    float m_updateTimer = 0.0f;

    float m_targetPixelRatio;

    [SerializeField] UnityEvent m_fadeInFinishEvent;
    [SerializeField] UnityEvent m_fadeOutFinishEvent;

    private void Awake()
    {
        BeginFadeIn();
    }

    private void Start()
    {
        m_targetPixelRatio = m_pixelController.pixelRatio;
        m_pixelController.pixelRatio = m_beginPixelRatio;
        m_pixelController.UpdateLastCamSize();
    }

    // Update is called once per frame
    void Update()
    {
        m_updateDelegate.Invoke();
    }

    public void BeginFadeIn()
    {
        m_updateTimer = 0.0f;
        m_updateDelegate = UpdateFadeIn;
    }

    public void BeginFadeOut()
    {
        m_updateTimer = 0.0f;
        m_updateDelegate = UpdateFadeOut;
    }

    void UpdateFadeIn()
    {
        m_updateTimer += Time.deltaTime;
        if (m_updateTimer > m_fadeInTime)
        {
            m_updateTimer -= m_fadeInTime;
            m_updateDelegate = Empty;

            m_pixelController.pixelRatio = Mathf.Lerp(m_beginPixelRatio, m_targetPixelRatio, 1.0f);

            m_fadeInFinishEvent.Invoke();
        }
        else
        {
            m_pixelController.pixelRatio = Mathf.Lerp(m_beginPixelRatio, m_targetPixelRatio, m_updateTimer / m_fadeInTime);
        }
    }

    void UpdateFadeOut()
    {
        m_updateTimer += Time.deltaTime;
        if (m_updateTimer > m_fadeOutTime)
        {
            m_updateTimer -= m_fadeOutTime;
            m_updateDelegate = Empty;

            m_pixelController.pixelRatio = Mathf.Lerp(m_targetPixelRatio, m_beginPixelRatio, 1.0f);

            m_fadeOutFinishEvent.Invoke();
        }
        else
        {
            m_pixelController.pixelRatio = Mathf.Lerp(m_targetPixelRatio, m_beginPixelRatio, m_updateTimer / m_fadeOutTime);
        }
    }

    void Empty()
    {

    }
}
