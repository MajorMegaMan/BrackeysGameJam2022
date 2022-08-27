using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputReceiver inputReceiver = null;

    [SerializeField] CharacterController m_characterControl = null;

    [SerializeField] float m_speed = 5.0f;

    Vector3 m_moveInput = Vector3.zero;

    Vector3 m_velocity = Vector3.zero;
    float m_currentSpeed = 0.0f;

    // Ground Checking
    [SerializeField] float m_groundRayDistance = 0.02f;
    [SerializeField] float m_gravity = Physics.gravity.y;
    [SerializeField] LayerMask m_groundLayer = ~0;

    Vector3 m_groundNormal = Vector3.up;

    float m_verticalVelocity = 0.0f;

    public float speed { get { return m_speed; } }
    public Vector3 velocity { get { return m_velocity; } }
    public float currentSpeed { get { return m_currentSpeed; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        bool isGrounded = false;
        if(!CheckForGround(out RaycastHit hitInfo))
        {
            m_verticalVelocity += m_gravity * Time.deltaTime;
        }
        else
        {
            m_verticalVelocity = 0.0f;
            m_groundNormal = hitInfo.normal;
            isGrounded = true;
        }

        Vector3 moveDir = m_moveInput;
        if (isGrounded)
        {
            //moveDir = Vector3.ProjectOnPlane(moveDir, m_groundNormal);
        }

        m_velocity = moveDir * m_speed;

        Vector3 moveVector = m_velocity + Vector3.up * m_verticalVelocity;
        m_currentSpeed = m_velocity.magnitude;

        m_characterControl.Move(moveVector * Time.deltaTime);
    }

    Vector3 CalculateHeightOffset()
    {
        float halfHeight = m_characterControl.height * 0.5f;
        Vector3 heightOffset = Vector3.up * (halfHeight - m_characterControl.radius);
        return heightOffset;
    }

    bool CheckForGround(out RaycastHit hitInfo)
    {
        Vector3 heightOffset = CalculateHeightOffset();

        Vector3 bottom = m_characterControl.center - heightOffset;
        Vector3 top = m_characterControl.center + heightOffset;
        bottom += transform.position;
        top += transform.position;

        return Physics.CapsuleCast(bottom, top, m_characterControl.radius, Vector3.down, out hitInfo, m_groundRayDistance + m_characterControl.skinWidth, m_groundLayer, QueryTriggerInteraction.Ignore);
    }

    void GetInputs()
    {
        m_moveInput = inputReceiver.GetMovement();
    }
}
