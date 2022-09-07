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
    Vector3 m_heading = Vector3.forward;

    // Ground Checking
    [SerializeField] float m_groundRayDistance = 0.02f;
    [SerializeField] float m_gravity = Physics.gravity.y;
    [SerializeField] LayerMask m_groundLayer = ~0;

    Vector3 m_groundNormal = Vector3.up;

    float m_verticalVelocity = 0.0f;

    PackagedStateMachine<PlayerController> m_groundedStateMachine;
    RaycastHit m_lastGroundHit;

    public float speed { get { return m_speed; } }
    public Vector3 velocity { get { return m_velocity; } }
    public float currentSpeed { get { return m_currentSpeed; } }
    public Vector3 heading { get { return m_heading; } }

    // totalVelocity is the movement velocity + vertical Velocity
    public Vector3 totalVelocity { get { return m_velocity + Vector3.up * m_verticalVelocity; } }

    public Vector3 groundNormal { get { return m_groundNormal; } }

    public bool isGrounded { get { return m_groundedStateMachine.GetCurrentState() == GroundedStateEnum.grounded; } }

    private void Awake()
    {
        var enumArray = System.Enum.GetValues(typeof(GroundedStateEnum));
        IState<PlayerController>[] groundedStates = new IState<PlayerController>[enumArray.Length];
        groundedStates[(int)GroundedStateEnum.grounded] = new GroundedState();
        groundedStates[(int)GroundedStateEnum.airborne] = new AirborneState();

        m_groundedStateMachine = new PackagedStateMachine<PlayerController>(this, groundedStates);

        if(CheckForGroundUpdate())
        {
            m_groundedStateMachine.InitialiseState(GroundedStateEnum.grounded);
        }
        else
        {
            m_groundedStateMachine.InitialiseState(GroundedStateEnum.airborne);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        m_groundedStateMachine.Invoke();

        Vector3 moveDir = m_moveInput;
        if (isGrounded)
        {
            // Redirect movement based on the ground normal
            Quaternion normalRot = Quaternion.FromToRotation(Vector3.up, m_groundNormal);
            moveDir = normalRot * moveDir;
        }

        m_velocity = moveDir * m_speed;

        Vector3 moveVector = m_velocity + Vector3.up * m_verticalVelocity;
        m_currentSpeed = m_velocity.magnitude;

        if(m_currentSpeed != 0.0f)
        {
            m_heading = m_velocity / m_currentSpeed;
        }

        m_characterControl.Move(moveVector * Time.deltaTime);
    }

    Vector3 CalculateHeightOffset()
    {
        float halfHeight = m_characterControl.height * 0.5f;
        Vector3 heightOffset = Vector3.up * Mathf.Max((halfHeight - m_characterControl.radius), 0.0f); // height offset cannot be negative.
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

    // checks for ground and updates the last ground hit
    bool CheckForGroundUpdate()
    {
        bool groundCheck = CheckForGround(out RaycastHit hitInfo);
        m_lastGroundHit = hitInfo;
        return groundCheck;
    }

    void GetInputs()
    {
        m_moveInput = inputReceiver.GetMovement();
    }

    #region GroundedStates

    public enum GroundedStateEnum
    {
        grounded,
        airborne
    }

    class GroundedState : IState<PlayerController>
    {
        void IState<PlayerController>.Enter(PlayerController owner)
        {
            owner.m_verticalVelocity = 0.0f;
            owner.m_groundNormal = owner.m_lastGroundHit.normal;

            // Force the character to collide with the ground and snap to the skin width
            owner.m_characterControl.Move(-owner.m_groundNormal * owner.m_characterControl.skinWidth);
        }

        void IState<PlayerController>.Exit(PlayerController owner)
        {

        }

        void IState<PlayerController>.Invoke(PlayerController owner)
        {
            if (owner.CheckForGroundUpdate())
            {
                owner.m_groundNormal = owner.m_lastGroundHit.normal;
            }
            else
            {
                owner.m_verticalVelocity += owner.m_gravity * Time.deltaTime;
                owner.m_groundedStateMachine.ChangeToState(GroundedStateEnum.airborne);
            }
        }
    }

    class AirborneState : IState<PlayerController>
    {
        void IState<PlayerController>.Enter(PlayerController owner)
        {
            owner.m_groundNormal = Vector3.up;
        }

        void IState<PlayerController>.Exit(PlayerController owner)
        {

        }

        void IState<PlayerController>.Invoke(PlayerController owner)
        {
            if (owner.CheckForGroundUpdate())
            {
                if (owner.m_lastGroundHit.distance < owner.m_characterControl.skinWidth + owner.m_groundRayDistance)
                {
                    owner.m_groundedStateMachine.ChangeToState(GroundedStateEnum.grounded);
                }
                else
                {
                    owner.m_verticalVelocity += owner.m_gravity * Time.deltaTime;
                }
            }
            else
            {
                owner.m_verticalVelocity += owner.m_gravity * Time.deltaTime;
            }
        }
    }

    #endregion // ! GroundedStates
}

public static class PackagedSMExtensionPlayer
{
    public static void InitialiseState(this PackagedStateMachine<PlayerController> packagedStateMachine, PlayerController.GroundedStateEnum selectionState)
    {
        packagedStateMachine.InitialiseState((int)selectionState);
    }

    public static void ChangeToState(this PackagedStateMachine<PlayerController> packagedStateMachine, PlayerController.GroundedStateEnum selectionState)
    {
        packagedStateMachine.ChangeToState((int)selectionState);
    }

    public static PlayerController.GroundedStateEnum GetCurrentState(this PackagedStateMachine<PlayerController> packagedStateMachine)
    {
        return (PlayerController.GroundedStateEnum)packagedStateMachine.currentIndex;
    }
}