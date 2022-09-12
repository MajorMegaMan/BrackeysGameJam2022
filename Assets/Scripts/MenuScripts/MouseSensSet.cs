using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensSet : MonoBehaviour
{
    [SerializeField] CameraSpin m_cameraSpin;
    [SerializeField] Slider m_camSpeedSlider;
    [SerializeField] float m_camSpeedMin = 0.2f;
    [SerializeField] float m_camSpeedMax = 2.0f;

    [SerializeField] SimpleFollow m_camFollow;
    [SerializeField] Slider m_camDistanceSlider;
    [SerializeField] float m_camDistanceMin = 3.0f;
    [SerializeField] float m_camDistanceMax = 8.0f;

    [SerializeField] Toggle m_invertXToggle;
    [SerializeField] Toggle m_invertYToggle;

    [SerializeField] ScriptableMouseSettings m_savedSettings;

    static bool _isLoaded = false;

    private void Start()
    {
        if (!_isLoaded)
        {
            _isLoaded = true;
            m_savedSettings.LoadSettings();
        }

        if (m_cameraSpin != null)
        {
            m_cameraSpin.camSpeed = m_savedSettings.mouseSens;
        }

        InitSlider(m_camSpeedSlider, m_camSpeedMin, m_camSpeedMax, m_savedSettings.mouseSens, SetCamSpeedLevel);

        if(m_camFollow != null)
        {
            m_camFollow.distance = m_savedSettings.camDistance;
        }

        InitSlider(m_camDistanceSlider, m_camDistanceMin, m_camDistanceMax, m_savedSettings.camDistance, SetCamDistanceLevel);

        m_invertXToggle.isOn = m_savedSettings.invertX;
        m_invertYToggle.isOn = m_savedSettings.invertY;

        m_invertXToggle.onValueChanged.AddListener(SetInvertX);
        m_invertYToggle.onValueChanged.AddListener(SetInvertY);
    }

    void InitSlider(Slider slider, float min, float max, float current, UnityEngine.Events.UnityAction<float> onChange)
    {
        slider.minValue = min;
        slider.maxValue = max;
        slider.value = current;

        slider.onValueChanged.AddListener(onChange);
    }

    void SetCamSpeedLevel(float sliderValue)
    {
        m_savedSettings.mouseSens = sliderValue;
        if (m_cameraSpin != null)
        {
            m_cameraSpin.camSpeed = m_savedSettings.mouseSens;
        }
    }

    void SetCamDistanceLevel(float sliderValue)
    {
        m_savedSettings.camDistance = sliderValue;
        if (m_camFollow != null)
        {
            m_camFollow.distance = m_savedSettings.camDistance;
        }
    }

    void SetInvertX(bool toggleValue)
    {
        m_savedSettings.invertX = toggleValue;
        if (m_cameraSpin != null)
        {
            m_cameraSpin.invertX = m_savedSettings.invertX;
        }
    }

    void SetInvertY(bool toggleValue)
    {
        m_savedSettings.invertY = toggleValue;
        if (m_cameraSpin != null)
        {
            m_cameraSpin.invertY = m_savedSettings.invertY;
        }
    }

    private void OnApplicationQuit()
    {
        m_savedSettings.SaveSettings();
    }
}
