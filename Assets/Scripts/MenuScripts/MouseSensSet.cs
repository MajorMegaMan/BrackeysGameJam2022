using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensSet : MonoBehaviour
{
    [SerializeField] CameraSpin m_cameraSpin;
    [SerializeField] Slider m_slider;
    [SerializeField] float m_min = 0.2f;
    [SerializeField] float m_max = 2.0f;

    [SerializeField] ScriptableMouseSettings m_savedSettings;

    static bool _isLoaded = false;

    private void Start()
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            m_savedSettings.mouseSens = PlayerPrefs.GetFloat("MouseSens", m_savedSettings.mouseSens);
        }

        if (m_cameraSpin != null)
        {
            m_cameraSpin.camSpeed = m_savedSettings.mouseSens;
        }

        m_slider.minValue = m_min;
        m_slider.maxValue = m_max;
        m_slider.value = m_savedSettings.mouseSens;

        m_slider.onValueChanged.AddListener(SetLevel);
    }

    void SetLevel(float sliderValue)
    {
        m_savedSettings.mouseSens = sliderValue;
        if (m_cameraSpin != null)
        {
            m_cameraSpin.camSpeed = m_savedSettings.mouseSens;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("MouseSens", m_savedSettings.mouseSens);
    }
}
