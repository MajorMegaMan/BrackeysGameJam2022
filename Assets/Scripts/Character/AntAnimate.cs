using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntAnimate : MonoBehaviour
{
    [SerializeField] CharacterAnimate m_characterAnimate = null;
    [SerializeField] AntBoid m_ant = null;

    [SerializeField] Transform m_modelTransform = null;

    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 heading = m_ant.velocity;
        heading.y = 0;
        if (heading.sqrMagnitude != 0)
        {
            m_modelTransform.forward = heading;
        }
        m_characterAnimate.SetMovementParameter(m_ant.currentSpeed / m_ant.speed);
    }
}
