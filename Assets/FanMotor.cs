using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanMotor : MonoBehaviour
{
    [SerializeField] Rigidbody m_rigidbody;

    [SerializeField] float m_force = 5.0f;
    [SerializeField] float m_maxRotationSpeed = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody.maxAngularVelocity = Mathf.Deg2Rad * m_maxRotationSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_rigidbody.AddTorque(Vector3.right * m_force);
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            m_rigidbody.maxAngularVelocity = Mathf.Deg2Rad * m_maxRotationSpeed;
        }
    }
}
