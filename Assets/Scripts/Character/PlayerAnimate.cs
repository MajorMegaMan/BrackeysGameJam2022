using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    [SerializeField] CharacterAnimate m_characterAnimate = null;
    [SerializeField] PlayerController m_player = null;

    [SerializeField] Transform m_modelTransform = null;

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 heading = m_player.velocity;
        heading.y = 0;
        if(heading.sqrMagnitude != 0)
        {
            m_modelTransform.forward = heading;
        }
        m_characterAnimate.SetMovementParameter(m_player.currentSpeed / m_player.speed);
    }
}
