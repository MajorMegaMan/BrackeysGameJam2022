using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script's only purpose is to disable the audiolistener that belongs to the background music.
public class MusicStartDisabler : MonoBehaviour
{
    private void Start()
    {
        BackgroundMusic.instance.EnableAudioListener(false);
    }
}
