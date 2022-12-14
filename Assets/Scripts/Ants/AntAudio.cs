using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntAudio : MonoBehaviour
{
    [SerializeField] AntBoid m_ant = null;
    [SerializeField] AudioSource m_audioSource = null;

    float m_footTimer = 0.0f;

    float m_randomTimerTarget = 0.0f;

    float m_basePitch;

    AntSettings settings { get { return AntManager.instance.settings; } }

    static float _currentPlayCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_basePitch = m_audioSource.pitch;
        UpdateRandomTimerTarget();
    }

    // Update is called once per frame
    void Update()
    {
        m_footTimer += m_ant.currentSpeed * Time.deltaTime;
        if (m_footTimer > m_randomTimerTarget)
        {
            m_footTimer -= m_randomTimerTarget;
            UpdateRandomTimerTarget();


            _currentPlayCount--;
            if(_currentPlayCount < 0)
            {
                _currentPlayCount = 0;
            }
            if(CheckNoisePollution())
            {
                PlayFootSound();
            }
        }
    }

    // returns true if the sound is able to play
    bool CheckNoisePollution()
    {
        if(_currentPlayCount == 0)
        {
            return true;
        }

        float roof = Mathf.Pow(_currentPlayCount, settings.audioPollutionExponent);
        return Random.Range(0.0f, roof) < _currentPlayCount;
    }

    void UpdateRandomTimerTarget()
    {
        m_randomTimerTarget = settings.footPlayFrequency + Random.Range(0.0f, settings.footStepRandomTimeStep);
    }

    void RandomizePitch()
    {
        float rand = Random.Range(-settings.halfPitchRange, settings.halfPitchRange);
        m_audioSource.pitch = m_basePitch + rand;
    }

    public void PlayFootSound()
    {
        _currentPlayCount += settings.audioPollutionCount;
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
