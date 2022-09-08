using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAntSettings", menuName = "ScriptableObjects/AntSettings")]
public class AntSettings : ScriptableObject
{
    [Header("Player Variables")]
    // Range that the player should enter for the ants to start following the player.
    [SerializeField] float m_pickUpRadius = 3.0f;

    // Relative distance that ants will try to follow the player.
    [SerializeField] float m_followDistance = 2.0f;
    [SerializeField] float m_followPathResetInterval = 0.2f;

    [Header("Building Variables")]
    // The height the ant will be considered when trying to build a bridge or ladder.
    [SerializeField] float m_singleBuildHeight = 2.0f;

    [SerializeField] float m_buildArrivedistance = 1.0f;

    [SerializeField] float m_buildRotationSpeed = 5.0f;
    [SerializeField] LayerMask m_environmentObstuctionLayerMask = ~0;

    [Header("Climb Variables")]
    [SerializeField] float m_climbSpeed = 5.0f;
    
    // The distance the min distance from the target climb position before the ant will snap to position
    [SerializeField] float m_climbSnapDistance = 0.05f;

    [SerializeField] float m_bridgeOffsetDistance = 0.5f;

    [Header("Ragdoll Variables")]
    [SerializeField] float m_ragdollRestTime = 2.0f;
    [SerializeField] float m_ragdollMinSpeedThreshold = 0.05f;

    [Header("Carry Variables")]
    [SerializeField] float m_carrySnapDistance = 0.1f;

    [Header("Audio")]
    [SerializeField] AudioClip[] m_positiveAntSounds = null;
    [SerializeField] AudioClip[] m_negativeAntSounds = null;
    [SerializeField] AudioClip[] m_footSounds = null;

    [SerializeField] float m_antSoundVolume = 1.0f;

    [SerializeField] float m_footPlayFrequency = 0.8f;
    [SerializeField] float m_footStepRandomTimeStep = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] float m_footVolume = 0.3f;

    [SerializeField] float m_audioPollutionCount = 2.0f;
    [SerializeField] float m_audioPollutionExponent = 2.0f;

    [SerializeField] float m_halfPitchRange = 0.2f;

    public float pickUpRadius { get { return m_pickUpRadius; } }
    public float followDistance { get { return m_followDistance; } }
    public float followPathResetInterval { get { return m_followPathResetInterval; } }

    public float singleBuildHeight { get { return m_singleBuildHeight; } }
    public float buildArriveDistance { get { return m_buildArrivedistance; } }
    public float buildRotationSpeed { get { return m_buildRotationSpeed; } }
    public LayerMask environmentObstuctionLayerMask { get { return m_environmentObstuctionLayerMask; } }

    public float climbSpeed { get { return m_climbSpeed; } }
    public float climbFinishDistance { get { return m_climbSnapDistance; } }
    public float bridgeOffsetDistance { get { return m_bridgeOffsetDistance; } }

    public float ragdollRestTime { get { return m_ragdollRestTime; } }
    public float ragdollMinSpeedThreshold { get { return m_ragdollMinSpeedThreshold; } }

    public float carrySnapDistance { get { return m_carrySnapDistance; } }

    #region Audio
    public AudioClip[] positiveAntSounds { get { return m_positiveAntSounds; } }
    public AudioClip[] negativeAntSounds { get { return m_negativeAntSounds; } }
    public AudioClip[] footSounds { get { return m_footSounds; } }
    public float antSoundVolume { get { return m_antSoundVolume; } }
    public float footPlayFrequency { get { return m_footPlayFrequency; } }
    public float footStepRandomTimeStep { get { return m_footStepRandomTimeStep; } }
    public float footVolume { get { return m_footVolume; } }
    public float audioPollutionCount { get { return m_audioPollutionCount; } }
    public float audioPollutionExponent { get { return m_audioPollutionExponent; } }
    public float halfPitchRange { get { return m_halfPitchRange; } }
    #endregion // !Audio
}
