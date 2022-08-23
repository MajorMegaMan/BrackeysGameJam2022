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

    [Header("Building Variables")]
    // The height the ant will be considered when trying to build a bridge or ladder.
    [SerializeField] float m_singleBuildHeight = 2.0f;

    [SerializeField] float m_buildArrivedistance = 1.0f;

    [SerializeField] float m_buildRotationSpeed = 5.0f;

    [Header("Climb Variables")]
    [SerializeField] float m_climbSpeed = 5.0f;
    
    // The distance the min distance from the target climb position before the ant will snap to position
    [SerializeField] float m_climbSnapDistance = 0.05f;

    [Header("Ragdoll Variables")]
    [SerializeField] float m_ragdollRestTime = 2.0f;
    [SerializeField] float m_ragdollMinSpeedThreshold = 0.05f;

    [Header("Carry Variables")]
    [SerializeField] float m_carrySnapDistance = 0.1f;

    public float pickUpRadius { get { return m_pickUpRadius; } }
    public float followDistance { get { return m_followDistance; } }
    public float singleBuildHeight { get { return m_singleBuildHeight; } }
    public float buildArriveDistance { get { return m_buildArrivedistance; } }
    public float buildRotationSpeed { get { return m_buildRotationSpeed; } }
    public float climbSpeed { get { return m_climbSpeed; } }
    public float climbFinishDistance { get { return m_climbSnapDistance; } }
    public float ragdollRestTime { get { return m_ragdollRestTime; } }
    public float ragdollMinSpeedThreshold { get { return m_ragdollMinSpeedThreshold; } }
    public float carrySnapDistance { get { return m_carrySnapDistance; } }
}
