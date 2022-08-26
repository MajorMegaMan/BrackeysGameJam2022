using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] PlayerController m_player = null;

    [SerializeField] AudioSource m_audioSource = null;

    float m_footTimer = 0.0f;

    float m_basePitch;

    AntSettings settings { get { return AntManager.instance.settings; } }

    // Start is called before the first frame update
    void Start()
    {
        m_basePitch = m_audioSource.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        m_footTimer += m_player.currentSpeed * Time.deltaTime;
        if(m_footTimer > settings.footPlayFrequency)
        {
            m_footTimer -= settings.footPlayFrequency;

            PlayFootSound();
        }
    }

    void RandomizePitch()
    {
        float rand = Random.Range(-settings.halfPitchRange, settings.halfPitchRange);
        m_audioSource.pitch = m_basePitch + rand;
    }

    public void PlayFootSound()
    {
        PlayRandomAudio(settings.footSounds, settings.footVolume);
    }

    public void PlayPositiveAntSound()
    {
        PlayRandomAudio(settings.positiveAntSounds, settings.antSoundVolume);
    }

    public void PlayNegativeAntSound()
    {
        PlayRandomAudio(settings.negativeAntSounds, settings.antSoundVolume);
    }

    void PlayRandomAudio(AudioClip[] clipArray, float volume)
    {
        RandomizePitch();
        int rand = Random.Range(0, clipArray.Length);
        m_audioSource.PlayOneShot(clipArray[rand], volume);
    }
}
