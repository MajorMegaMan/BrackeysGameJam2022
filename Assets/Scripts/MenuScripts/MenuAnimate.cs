using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuAnimate : MonoBehaviour
{
    [SerializeField] RectTransform m_animatedRectTransform = null;

    [SerializeField] RectTransform m_startTransform = null;
    [SerializeField] RectTransform m_endTransform = null;

    [SerializeField] AnimationCurve m_enterLerpCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] AnimationCurve m_exitLerpCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    float m_time = 0.0f;

    [SerializeField] float m_length = 2.0f;
    [SerializeField] float m_randMin = 0.5f;
    [SerializeField] float m_randMax = 1.0f;

    delegate void AnimateDelegate();
    AnimateDelegate m_animateDelegate;

    [SerializeField] UnityEvent m_onArriveStartEvent = null;
    [SerializeField] UnityEvent m_onArriveEndEvent = null;

    public UnityEvent onArriveStartEvent { get { return m_onArriveStartEvent; } }
    public UnityEvent onArriveEndEvent { get { return m_onArriveStartEvent; } }

    // 0 is no direction, 1 is towards end, -1 is towards start
    int m_travelDirection = 0;

    // Start is called before the first frame update
    void Start()
    {
        // initialise if it has not already been set.
        if(m_travelDirection == 0)
        {
            m_animateDelegate = Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_animateDelegate.Invoke();
    }

    void EnterLerp(float t)
    {
        m_animatedRectTransform.anchoredPosition = Vector3.LerpUnclamped(m_startTransform.anchoredPosition, m_endTransform.anchoredPosition, m_enterLerpCurve.Evaluate(t));
    }

    void ExitLerp(float t)
    {
        m_animatedRectTransform.anchoredPosition = Vector3.LerpUnclamped(m_endTransform.anchoredPosition, m_startTransform.anchoredPosition, m_exitLerpCurve.Evaluate(t));
    }

    public void SetToStart()
    {
        m_time = 0.0f;
        EnterLerp(0.0f);
    }

    public void SetToEnd()
    {
        m_time = 0.0f;
        ExitLerp(0.0f);
    }

    public void BeginEnterAnimate()
    {
        m_time = 0.0f;
        EnterLerp(0.0f);
        m_animateDelegate = EnterAnimate;
        m_travelDirection = 1;
    }

    public void BeginExitAnimate()
    {
        m_time = 0.0f;
        ExitLerp(0.0f);
        m_animateDelegate = ExitAnimate;
        m_travelDirection = -1;
    }

    void EnterAnimate()
    {
        m_time += (Time.deltaTime / m_length) * Random.Range(m_randMin, m_randMax);
        if(m_time < 1.0f)
        {
            EnterLerp(m_time);
        }
        else
        {
            EnterLerp(1.0f);
            m_animateDelegate = Empty;
            m_travelDirection = 0;
            m_onArriveEndEvent.Invoke();
        }
    }

    void ExitAnimate()
    {
        m_time += (Time.deltaTime / m_length) * Random.Range(m_randMin, m_randMax);
        if (m_time < 1.0f)
        {
            ExitLerp(m_time);
        }
        else
        {
            ExitLerp(1.0f);
            m_animateDelegate = Empty;
            m_travelDirection = 0;
            m_onArriveStartEvent.Invoke();
        }
    }

    void Empty()
    {

    }
}
