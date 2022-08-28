using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMixerVolumes", menuName = "Scriptables/Audio/MixerVolumes")]
public class ScriptableMixerVolumes : ScriptableObject
{
    public float masterValue = 0.7f;
    public float musicValue = 0.7f;
    public float sfxValue = 0.7f;
}
