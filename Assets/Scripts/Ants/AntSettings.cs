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


    public float pickUpRadius { get { return m_pickUpRadius; } }
    public float followDistance { get { return m_followDistance; } }
}
