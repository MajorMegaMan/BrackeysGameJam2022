using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimate : MonoBehaviour
{
    [SerializeField] CharacterAnimate m_characterAnimate = null;
    [SerializeField] PlayerController m_player = null;

    [SerializeField] Transform m_modelTransform = null;

    [SerializeField] string m_danceState = "Ant_Dance";

    Vector3 m_smoothLookDir = Vector3.forward;
    Vector3 m_smoothLookVel = Vector3.zero;
    [SerializeField] float m_smoothLookTime = 0.08f;

    [SerializeField, Range(0.0f, 1.0f)] float m_aerialSlerp = 0.5f;

    public float aerialSlerp { get { return m_aerialSlerp; } set { m_aerialSlerp = Mathf.Clamp(value, 0.0f, 1.0f); } }

    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 lookDir;
        if(m_player.isGrounded)
        {
            lookDir = m_player.heading;
        }
        else
        {
            lookDir = Vector3.Slerp(m_player.heading, m_player.totalVelocity.normalized, m_aerialSlerp);
        }

        if(lookDir.sqrMagnitude != 0)
        {
            m_smoothLookDir = Vector3.SmoothDamp(m_smoothLookDir, lookDir, ref m_smoothLookVel, m_smoothLookTime);
            m_modelTransform.LookAt(m_modelTransform.position + m_smoothLookDir);
        }
        m_characterAnimate.SetMovementParameter(m_player.currentSpeed / m_player.speed);
    }

    public void Dance()
    {
        m_characterAnimate.CrossFade(m_danceState, 0.1f);
    }
}
