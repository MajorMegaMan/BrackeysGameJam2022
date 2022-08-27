using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener), typeof(AudioSource))]
public class BackgroundMusic : LazyMonoSingletonBase<BackgroundMusic>
{
    AudioSource m_audioSource;
    AudioListener m_selfListener;

    public AudioSource audioSource { get { return m_audioSource; } }

    [SerializeField] float m_halfPitchRange = 0.02f;
    float m_basePitch;

    private void Awake()
    {
        if(instance != this)
        {
            Destroy(gameObject);
        }

        m_selfListener = GetComponent<AudioListener>();
        m_audioSource = GetComponent<AudioSource>();

        m_basePitch = audioSource.pitch;
    }

    private void Update()
    {
        float sinTime = Mathf.Sin(Time.time);
        audioSource.pitch = m_basePitch + sinTime * m_halfPitchRange;
    }

    public void EnableAudioListener(bool enabled)
    {
        m_selfListener.enabled = enabled;
    }
}
