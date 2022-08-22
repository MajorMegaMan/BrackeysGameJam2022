using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AntBoid : MonoBehaviour
{
    // Navigation
    [SerializeField] NavMeshAgent m_navAgent;

    Vector3 m_navTarget = Vector3.zero;
    Transform m_followTarget = null;

    StateMachine<AntBoid> m_actionStateMachine;
    IState<AntBoid>[] m_actionStates;

    // Gameplay
    [SerializeField] PickUpTrigger m_pickUpTrigger = null;

    // Building
    AntBuildingGroup m_currentBuildGroup = null;
    Vector3 m_climbPosition = Vector3.zero;

    // Getters
    PlayerController player { get { return AntManager.instance.player; } }
    AntSettings settings { get { return AntManager.instance.settings; } }

    [SerializeField] MeshRenderer debugRenderer = null;
    Material debugMaterial { get; set; }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseStateMachine();

        debugMaterial = debugRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        m_actionStateMachine.Invoke();
    }

    // Should be called by the pick up Trigger event
    public void PlayerPickUpTriggerEvent()
    {
        SetState(StateEnum.following);
    }

    // Should be called when the player does not "own" the ant anymore
    public void PlayerDropTriggerEvent()
    {
        SetState(StateEnum.empty);
    }

    public void SendToBuildCell(GridManager.GridCellKey gridCellKey)
    {
        SetNavTarget(GameManager.instance.gridManager.GetCellFloorPosition(gridCellKey));
        SetState(StateEnum.buildMove);
    }

    public void SetBuildGroup(AntBuildingGroup antBuildingGroup)
    {
        m_currentBuildGroup = antBuildingGroup;

        if(m_currentBuildGroup != null)
        {
            debugMaterial.color = Color.red;

            m_currentBuildGroup.AddAntToGroup(this);

            SetNavTarget(antBuildingGroup.start);
            SetState(StateEnum.buildMove);
        }
        else
        {
            debugMaterial.color = Color.white;
            SetState(StateEnum.empty);
        }
    }

    public void StartClimbing(Vector3 climbPosiiton)
    {
        m_climbPosition = climbPosiiton;
        SetState(StateEnum.climbing);
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
        Vector3 toTarget = m_followTarget.position - transform.position;
        SetNavTarget(m_followTarget.position - (toTarget.normalized * settings.followDistance));
        m_navAgent.SetDestination(m_navTarget);
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
        m_actionStateMachine.SetState(m_actionStates[(int)stateEnum]);
    }

    #region StatesInternals
    internal void EnterWaiting()
    {
        m_pickUpTrigger.gameObject.SetActive(true);
        SetFollowTarget(null);
        SetNavTarget(transform.position);

        AntManager.instance.RemoveFromPlayerGroup(this);
        AntManager.instance.RemoveFromBuildAnts(this);
    }

    internal void EnterFollowing()
    {
        m_pickUpTrigger.gameObject.SetActive(false);
        SetFollowTarget(player.transform);

        AntManager.instance.AddToPlayerGroup(this);
        AntManager.instance.RemoveFromBuildAnts(this);
    }

    internal void EnterBuilding()
    {
        m_pickUpTrigger.gameObject.SetActive(false);
        SetFollowTarget(null);
        m_navAgent.SetDestination(m_navTarget);

        AntManager.instance.RemoveFromPlayerGroup(this);
        AntManager.instance.AddToBuildAnts(this);
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
        frozen
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
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {

        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            owner.MoveFollow();
        }
    }

    class BuildingMoveState : IState<AntBoid>
    {
        GridManager.GridCellKey m_currentCellKey;

        void IState<AntBoid>.Enter(AntBoid owner)
        {
            owner.StartNavigating();
            m_currentCellKey = GameManager.instance.gridManager.GetCellKey(owner.m_navTarget);
            //GameManager.instance.gridManager.AddGridCellObject(m_currentCellKey, owner);
            owner.EnterBuilding();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            //GameManager.instance.gridManager.RemoveGridCellObject(m_currentCellKey);
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
            owner.debugMaterial.color = Color.blue;
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

            owner.debugMaterial.color = Color.yellow;

            Vector3 lineDir = owner.m_currentBuildGroup.GetLine().normalized;
            owner.transform.up = lineDir;
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {
            
        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            Vector3 toClimbPos = owner.m_climbPosition - owner.transform.position;
            float remain = toClimbPos.magnitude;

            float deltaMagnitude = owner.settings.climbSpeed * Time.deltaTime;

            if (remain < 0.001f)
            {
                // arrived
                Debug.Log("Climbing Arrived");
                owner.transform.position = owner.m_climbPosition;
                owner.SetState(StateEnum.frozen);
            }
            else
            {
                Vector3 lineDir = toClimbPos / remain;
                owner.transform.position += lineDir * deltaMagnitude;
                Debug.Log("Climbing");
            }
        }
    }

    class FrozenState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
            Debug.Log("Freeze");
            owner.StopNavigating();
            if(owner.m_currentBuildGroup != null)
            {
                owner.m_currentBuildGroup.FreezeAnt(owner);
            }

            owner.debugMaterial.color = Color.green;
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {

        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {

        }
    }

    #endregion // !States
}
