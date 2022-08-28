using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FanMotor : MonoBehaviour
{
    [SerializeField] Rigidbody m_rigidbody;

    [SerializeField] float m_force = 5.0f;
    [SerializeField] float m_maxRotationSpeed = 15.0f;

    public bool isTurning = true;

    [SerializeField] float m_blowUpImpulse = 50.0f;
    [SerializeField] UnityEvent m_blowUpEvent;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody.maxAngularVelocity = Mathf.Deg2Rad * m_maxRotationSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isTurning)
        {
            m_rigidbody.AddTorque(Vector3.right * m_force);
        }
    }

    public void SetIsTurning(bool value)
    {
        isTurning = value;
    }

    public void BlowUp()
    {
        m_rigidbody.constraints = RigidbodyConstraints.None;
        m_rigidbody.AddForce(-transform.right * m_blowUpImpulse, ForceMode.Impulse);
        m_blowUpEvent.Invoke();
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            m_rigidbody.maxAngularVelocity = Mathf.Deg2Rad * m_maxRotationSpeed;
        }
    }
}
