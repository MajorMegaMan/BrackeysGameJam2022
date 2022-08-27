using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationGroup : MonoBehaviour
{
    [SerializeField] MenuAnimate[] m_menuAnimateArray;

    [SerializeField] bool m_atStart = true;
    [SerializeField] bool m_isActive = true;
    [SerializeField] bool m_autoBegin = false;

    public bool isActive { get { return m_isActive; } }

    private void Awake()
    {
        if(m_atStart)
        {
            SetToStart();
            if (m_autoBegin)
            {
                BeginEnter();
            }
        }
        else
        {
            SetToEnd();
            if (m_autoBegin)
            {
                BeginExit();
            }
        }

        Activate(m_isActive);
    }

    public void Activate(bool isActive)
    {
        m_isActive = isActive;
        for (int i = 0; i < m_menuAnimateArray.Length; i++)
        {
            m_menuAnimateArray[i].gameObject.SetActive(isActive);
        }
    }

    public void SetToStart()
    {
        m_atStart = true;
        for (int i = 0; i < m_menuAnimateArray.Length; i++)
        {
            m_menuAnimateArray[i].SetToStart();
        }
    }

    public void SetToEnd()
    {
        m_atStart = false;
        for (int i = 0; i < m_menuAnimateArray.Length; i++)
        {
            m_menuAnimateArray[i].SetToEnd();
        }
    }

    public void BeginEnter()
    {
        if (!m_atStart)
        {
            return;
        }

        if (!m_isActive)
        {
            Activate(true);
        }

        for (int i = 0; i < m_menuAnimateArray.Length; i++)
        {
            m_menuAnimateArray[i].BeginEnterAnimate();
            m_atStart = false;
        }
    }

    public void BeginExit()
    {
        if (m_atStart)
        {
            return;
        }

        if (!m_isActive)
        {
            Activate(true);
        }

        for (int i = 0; i < m_menuAnimateArray.Length; i++)
        {
            m_menuAnimateArray[i].BeginExitAnimate();
            m_atStart = true;
        }
    }
}
