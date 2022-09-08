using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AntBoid : MonoBehaviour
{
    // Navigation
    [SerializeField] NavMeshAgent m_navAgent;

    Vector3 m_navTarget = Vector3.zero;
    Transform m_followTarget = null;
    float m_followPathResetTimer = 0.0f;

    StateMachine<AntBoid> m_actionStateMachine;
    IState<AntBoid>[] m_actionStates;
    StateEnum m_currentState = 0;

    // Gameplay
    [SerializeField] PickUpTrigger m_pickUpTrigger = null;
    [SerializeField] CollisionTrigger m_collisionTrigger = null;

    // Building
    AntBuildingGroup m_currentBuildGroup = null;
    Vector3 m_climbPosition = Vector3.zero;
    Vector3 m_climbDir = Vector3.zero;

    // Carrying
    CarryableObject m_currentCarryableObject = null;
    int m_carryPositionIndex = 0;
    bool m_isHolding = false;

    // Animation
    [SerializeField] AntAnimate m_antAnimator = null;
    [SerializeField] RagdollAnimator m_ragdoll = null;

    // Audio
    [SerializeField] AntAudio m_audio = null;

    // Getters
    PlayerController player { get { return AntManager.instance.player; } }
    AntSettings settings { get { return AntManager.instance.settings; } }

    delegate T NavGetter<T>();
    NavGetter<float> m_speedGetter;
    NavGetter<Vector3> m_velocityGetter;

    public float speed { get { return m_speedGetter.Invoke(); } }
    public Vector3 velocity { get { return m_velocityGetter.Invoke(); } }
    public float currentSpeed { get { return GetCurrentSpeed(); } }


    private void Awake()
    {
        SetNavGettersToSelf();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseStateMachine();

        AntManager.instance.AddToAllAnts(this);
    }

    private void OnDestroy()
    {
        var manager = AntManager.instance;
        if(manager != null)
        {
            AntManager.instance.RemoveFromAllAnts(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_actionStateMachine.Invoke();
    }

    #region Getters
    float GetNavSpeed()
    {
        return m_navAgent.speed;
    }

    Vector3 GetNavVelocity()
    {
        return m_navAgent.velocity;
    }

    float GetCurrentSpeed()
    {
        return m_velocityGetter.Invoke().magnitude;
    }

    float GetCarryNavSpeed()
    {
        return m_currentCarryableObject.speed;
    }

    Vector3 GetCarryNavVelocity()
    {
        return m_currentCarryableObject.velocity;
    }

    float GetClimbSpeed()
    {
        return settings.climbSpeed;
    }

    Vector3 GetClimbVelocity()
    {
        return m_climbDir * GetClimbSpeed();
    }

    void SetNavGettersToSelf()
    {
        m_speedGetter = GetNavSpeed;
        m_velocityGetter = GetNavVelocity;
    }

    void SetNavGettersToCarryObject()
    {
        m_speedGetter = GetCarryNavSpeed;
        m_velocityGetter = GetCarryNavVelocity;
    }

    void SetNavGettersToClimb()
    {
        m_speedGetter = GetClimbSpeed;
        m_velocityGetter = GetClimbVelocity;
    }

    #endregion // !Getters

    // Should be called by the pick up Trigger event
    public void PlayerPickUpTriggerEvent()
    {
        SetState(StateEnum.following);
    }

    // Should be called when the player does not "own" the ant anymore
    public void SetToWait()
    {
        SetState(StateEnum.empty);
    }

    public void SetBuildGroup(AntBuildingGroup antBuildingGroup)
    {
        m_currentBuildGroup = antBuildingGroup;

        if(m_currentBuildGroup != null)
        {
            m_currentBuildGroup.AddAntToGroup(this);

            SetNavTarget(antBuildingGroup.start);
            SetState(StateEnum.buildMove);
        }
        else
        {
            if(m_currentState == StateEnum.ragdoll)
            {
                // Do Nothing
            }
            else if(m_currentState == StateEnum.climbing || m_currentState == StateEnum.frozen)
            {
                SetState(StateEnum.ragdoll);
            }
            else
            {
                SetState(StateEnum.empty);
            }
        }
    }

    public void StartClimbing(Vector3 climbPosiiton)
    {
        m_climbPosition = climbPosiiton;
        SetState(StateEnum.climbing);
    }

    public void ActivateRagdoll()
    {
        SetState(StateEnum.ragdoll);
    }

    public void CarryObject(CarryableObject carryableObject, int positionIndex)
    {
        SetCarriableObject(carryableObject, positionIndex);
        SetState(StateEnum.carryMove);
    }

    void SetCarriableObject(CarryableObject carryableObject, int positionIndex)
    {
        if (m_currentCarryableObject != null)
        {
            m_currentCarryableObject.RemoveAnt(this);
        }

        m_currentCarryableObject = carryableObject;
        m_carryPositionIndex = positionIndex;

        if (m_currentCarryableObject != null)
        {
            m_currentCarryableObject.AddAnt(this);
        }
    }

    public void OnCollisionTrigger(Collider other)
    {
        var otherRigid = other.GetComponent<Rigidbody>();
        if(otherRigid != null)
        {
            ActivateRagdoll();
            m_ragdoll.GetRigidbody(0).AddForce(otherRigid.velocity, ForceMode.Impulse);
        }
    }

    #region Navigation

    void StartNavigating()
    {
        m_navAgent.enabled = true;

        m_navAgent.updatePosition = true;
        m_navAgent.updateRotation = true;
    }

    void StopNavigating()
    {
        m_navAgent.updatePosition = false;
        m_navAgent.updateRotation = false;

        m_navAgent.enabled = false;
    }

    // Called during the update step of FollowState
    void MoveFollow()
    {
        m_followPathResetTimer += Time.deltaTime;
        if(m_followPathResetTimer > settings.followPathResetInterval)
        {
            m_followPathResetTimer -= settings.followPathResetInterval;

            Vector3 toTarget = m_followTarget.position - transform.position;
            SetNavTarget(m_followTarget.position - (toTarget.normalized * settings.followDistance));
            m_navAgent.SetDestination(m_navTarget);


            if (m_navAgent.pathStatus == NavMeshPathStatus.PathPartial || m_navAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                // failed to reach player with a path
                SetToWait();
            }
        }
    }

    void SetNavTarget(Vector3 navTarget)
    {
        m_navTarget = navTarget;
    }

    void SetFollowTarget(Transform followTarget)
    {
        m_followTarget = followTarget;
    }
    #endregion // !Navigation

    #region States

    void SetState(StateEnum stateEnum)
    {
        m_currentState = stateEnum;
        m_actionStateMachine.SetState(m_actionStates[(int)stateEnum]);
    }

    #region StatesInternals
    internal void EnterWaiting()
    {
        m_pickUpTrigger.gameObject.SetActive(true);
        SetFollowTarget(null);
        SetNavTarget(transform.position);
    }

    internal void EnterFollowing()
    {
        m_pickUpTrigger.gameObject.SetActive(false);
        SetFollowTarget(player.transform);
        m_followPathResetTimer = 0.0f;

        Vector3 toTarget = m_followTarget.position - transform.position;
        SetNavTarget(m_followTarget.position - (toTarget.normalized * settings.followDistance));
        m_navAgent.SetDestination(m_navTarget);
    }

    internal void EnterBuilding()
    {
        m_pickUpTrigger.gameObject.SetActive(false);
        SetFollowTarget(null);
        m_navAgent.SetDestination(m_navTarget);
    }

    internal void EnterCarryMove()
    {
        SetNavTarget(m_currentCarryableObject.GetAntPosition(m_carryPositionIndex));
        m_navAgent.SetDestination(m_navTarget);
    }

    internal void EnterCarryHold()
    {
        StopNavigating();
        transform.forward = m_currentCarryableObject.GetAntCentre() - transform.position;
        m_currentCarryableObject.AntArrived(this);
    }

    #endregion // !StatesInternals

    void InitialiseStateMachine()
    {
        m_actionStateMachine = new StateMachine<AntBoid>(this);
        var enumArray = System.Enum.GetValues(typeof(StateEnum));
        m_actionStates = new IState<AntBoid>[enumArray.Length];
        m_actionStates[(int)StateEnum.empty] = new EmptyState();
        m_actionStates[(int)StateEnum.following] = new FollowingState();
        m_actionStates[(int)StateEnum.buildMove] = new BuildingMoveState();
        m_actionStates[(int)StateEnum.buildWait] = new BuildingWaitState();
        m_actionStates[(int)StateEnum.climbing] = new ClimbingState();
        m_actionStates[(int)StateEnum.frozen] = new FrozenState();
        m_actionStates[(int)StateEnum.ragdoll] = new RagdollState();
        m_actionStates[(int)StateEnum.carryMove] = new CarryMoveState();
        m_actionStates[(int)StateEnum.carryHold] = new CarryHoldState();

        m_currentState = StateEnum.empty;

        // This function must be called after awake as the ant manager needs to be initialised.
        m_actionStateMachine.InitialiseState(m_actionStates[(int)StateEnum.empty]);
    }

    enum StateEnum
    {
        empty,
        following,
        buildMove,
        buildWait,
        climbing,
        frozen,
        ragdoll,
        carryMove,
        carryHold
    }

    class EmptyState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StartNavigating();
            owner.EnterWaiting();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {

        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {

        }
    }

    class FollowingState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StartNavigating();
            owner.EnterFollowing();

            owner.m_audio.PlayPositiveAntSound();

            AntManager.instance.AddToPlayerGroup(owner);
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            AntManager.instance.RemoveFromPlayerGroup(owner);
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            owner.MoveFollow();
        }
    }

    class BuildingMoveState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StartNavigating();
            owner.EnterBuilding();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            if(owner.m_navAgent.remainingDistance < owner.settings.buildArriveDistance)
            {
                // arrived at build position
                owner.SetState(StateEnum.buildWait);
                owner.m_currentBuildGroup.AddToWaitingGroup(owner);
            }
        }
    }

    class BuildingWaitState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            
        }
    }

    class ClimbingState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StopNavigating();

            Vector3 lineDir = owner.m_currentBuildGroup.GetLine().normalized;
            owner.transform.forward = lineDir;

            owner.m_climbDir = lineDir;

            owner.m_antAnimator.Climb();

            owner.SetNavGettersToClimb();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            owner.m_antAnimator.Motion();
            owner.SetNavGettersToSelf();
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            Vector3 toClimbPos = owner.m_climbPosition - owner.transform.position;
            float remain = toClimbPos.magnitude;

            float deltaMagnitude = owner.settings.climbSpeed * Time.deltaTime;

            if (remain < owner.settings.climbFinishDistance)
            {
                // arrived
                owner.transform.position = owner.m_climbPosition;
                owner.SetState(StateEnum.frozen);
            }
            else
            {
                Vector3 lineDir = toClimbPos / remain;
                owner.transform.position += lineDir * deltaMagnitude;
            }
        }
    }

    class FrozenState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StopNavigating();
            if(owner.m_currentBuildGroup != null)
            {
                owner.m_currentBuildGroup.FreezeAnt(owner);
            }
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {

        }
    }

    class RagdollState : IState<AntBoid>
    {
        Rigidbody m_hips = null;
        Transform m_hipsTransform = null;
        Vector3 m_hipsOffset = Vector3.zero;

        float m_timer = 0.0f;

        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StopNavigating();
            owner.m_ragdoll.Activate(true);

            m_hips = owner.m_ragdoll.GetRigidbody(0);
            m_hipsTransform = m_hips.transform;

            m_hipsOffset = m_hipsTransform.position - owner.transform.position;

            m_timer = 0.0f;

            owner.m_collisionTrigger.gameObject.SetActive(false);

            if(owner.m_currentBuildGroup != null)
            {
                owner.m_currentBuildGroup.Dissassemble();
            }
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            Vector3 targetPosition = m_hipsTransform.position;
            owner.m_ragdoll.Activate(false);
            //Vector3 offset = m_hipsTransform.localToWorldMatrix * m_hipsOffset;
            //owner.transform.position = m_hipsTransform.position + offset;

            owner.StartNavigating();
            Vector3 position = targetPosition;
            owner.transform.position = position;
            owner.m_navAgent.Warp(position);

            owner.m_collisionTrigger.gameObject.SetActive(true);
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            if(m_hips.velocity.magnitude < owner.settings.ragdollMinSpeedThreshold)
            {
                m_timer += Time.deltaTime;
                if (m_timer > owner.settings.ragdollRestTime)
                {
                    owner.SetState(StateEnum.empty);
                }
            }
        }
    }

    class CarryMoveState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.EnterCarryMove();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            // Test the next state that the ant is moving into.
            if(owner.m_currentState != StateEnum.carryHold)
            {
                owner.SetCarriableObject(null, -1);
            }
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            if(owner.m_navAgent.remainingDistance < owner.settings.carrySnapDistance)
            {
                owner.transform.position = owner.m_navTarget;
                owner.SetState(StateEnum.carryHold);
            }
        }
    }

    class CarryHoldState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.EnterCarryHold();
            owner.SetNavGettersToCarryObject();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            owner.SetCarriableObject(null, -1);
            owner.SetNavGettersToSelf();
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            owner.transform.position = owner.m_currentCarryableObject.GetAntPosition(owner.m_carryPositionIndex);
        }
    }

    #endregion // !States

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5.0f);
    }
}
