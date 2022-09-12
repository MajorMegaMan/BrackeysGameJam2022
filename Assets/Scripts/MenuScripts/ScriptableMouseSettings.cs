using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMouseSettings", menuName = "Scriptables/Camera/Settings")]
public class ScriptableMouseSettings : ScriptableObject
{
    public float mouseSens = 1.0f;
    public float camDistance = 5.0f;
    [SerializeField] int m_invertX = 0;
    [SerializeField] int m_invertY = 0;

    public bool invertX
    {
        get { return m_invertX != 0; }
        set
        {
            if (value)
            {
                m_invertX = 1;
            }
            else
            {
                m_invertX = 0;
            }
        }
    }
    public bool invertY
    { 
        get { return m_invertY != 0; }
        set
        {
            if (value)
            {
                m_invertY = 1;
            }
            else
            {
                m_invertY = 0;
            }
        }
    }

    public void LoadSettings()
    {
        mouseSens = PlayerPrefs.GetFloat("MouseSens", mouseSens);
        camDistance = PlayerPrefs.GetFloat("CamDistance", camDistance);
        m_invertX = PlayerPrefs.GetInt("InvertX", m_invertX);
        m_invertY = PlayerPrefs.GetInt("InvertY", m_invertY);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSens);
        PlayerPrefs.SetFloat("CamDistance", camDistance);
        PlayerPrefs.SetInt("InvertX", m_invertX);
        PlayerPrefs.SetInt("InvertY", m_invertY);
    }
}
