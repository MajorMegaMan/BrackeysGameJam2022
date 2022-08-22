using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAntSettings", menuName = "ScriptableObjects/AntSettings")]
public class AntSettings : ScriptableObject
{
    // Range that the player should enter for the ants to start following the player.
    [SerializeField] float m_pickUpRadius = 3.0f;

    // Relative distance that ants will try to follow the player.
    [SerializeField] float m_followDistance = 2.0f;

    // The height the ant will be considered when trying to build a bridge or ladder.
    [SerializeField] float m_singleBuildHeight = 2.0f;

    [SerializeField] float m_buildArrivedistance = 1.0f;

    [SerializeField] float m_climbSpeed = 5.0f;

    [SerializeField] float m_buildRotationSpeed = 5.0f;

    public float pickUpRadius { get { return m_pickUpRadius; } }
    public float followDistance { get { return m_followDistance; } }
    public float singleBuildHeight { get { return m_singleBuildHeight; } }
    public float buildArriveDistance { get { return m_buildArrivedistance; } }
    public float climbSpeed { get { return m_climbSpeed; } }
    public float buildRotationSpeed { get { return m_buildRotationSpeed; } }
}
