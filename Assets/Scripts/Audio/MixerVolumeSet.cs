using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerVolumeSet : MonoBehaviour
{
    [SerializeField] AudioMixer m_mixer;
    [SerializeField] ScriptableMixerVolumes m_savedMixerVolumes;

    [SerializeField] Slider m_masterSlider;
    [SerializeField] Slider m_musicSlider;
    [SerializeField] Slider m_sfxSlider;

    private void Start()
    {
        m_masterSlider.value = m_savedMixerVolumes.masterValue;
        m_musicSlider.value = m_savedMixerVolumes.musicValue;
        m_sfxSlider.value = m_savedMixerVolumes.sfxValue;

        m_masterSlider.onValueChanged.AddListener(SetMasterLevel);
        m_musicSlider.onValueChanged.AddListener(SetMusicLevel);
        m_sfxSlider.onValueChanged.AddListener(SetSFXLevel);

        SetLevel("MasterVol", m_savedMixerVolumes.masterValue);
        SetLevel("MusicVol", m_savedMixerVolumes.musicValue);
        SetLevel("SFXVol", m_savedMixerVolumes.sfxValue);
    }

    public void SetMasterLevel(float sliderValue)
    {
        SetLevel("MasterVol", sliderValue);
        m_savedMixerVolumes.masterValue = sliderValue;
    }

    public void SetMusicLevel(float sliderValue)
    {
        SetLevel("MusicVol", sliderValue);
        m_savedMixerVolumes.musicValue = sliderValue;
    }

    public void SetSFXLevel(float sliderValue)
    {
        SetLevel("SFXVol", sliderValue);
        m_savedMixerVolumes.sfxValue = sliderValue;
    }

    void SetLevel(string name, float sliderValue)
    {
        if(sliderValue != 0.0f)
        {
            m_mixer.SetFloat(name, Mathf.Log10(sliderValue) * 20);
        }
        else
        {
            m_mixer.SetFloat(name, -80);
        }
    }
}
