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

    // Getters
    PlayerController player { get { return AntManager.instance.player; } }
    AntSettings settings { get { return AntManager.instance.settings; } }

    private void Awake()
    {
        m_actionStateMachine = new StateMachine<AntBoid>(this);
        m_actionStates = new IState<AntBoid>[3];
        m_actionStates[(int)StateEnum.waiting] = new WaitingState();
        m_actionStates[(int)StateEnum.following] = new FollowingState();
        m_actionStates[(int)StateEnum.building] = new BuildingState();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_actionStateMachine.InitialiseState(m_actionStates[(int)StateEnum.waiting]);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_followTarget != null)
        {
            Vector3 toTarget = m_followTarget.position - transform.position;

            SetNavTarget(m_followTarget.position - (toTarget.normalized * settings.followDistance));
        }

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

    // Should be called by the pick up Trigger event
    public void PlayerPickUpTriggerEvent()
    {
        SetState(StateEnum.following);
    }

    // Should be called when the player does not "own" the ant anymore
    public void PlayerDropTriggerEvent()
    {
        SetState(StateEnum.waiting);
    }

    public void SendToBuildCell(GridManager.GridCellKey gridCellKey)
    {
        SetNavTarget(GameManager.instance.gridManager.GetCellFloorPosition(gridCellKey));
        SetState(StateEnum.building);
    }

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

        AntManager.instance.RemoveFromPlayerGroup(this);
        AntManager.instance.AddToBuildAnts(this);
    }
    #endregion // !StatesInternals

    enum StateEnum
    {
        waiting,
        following,
        building
    }

    class WaitingState : IState<AntBoid>
    {
        void IState<AntBoid>.Enter(AntBoid owner)
        {
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
            owner.EnterFollowing();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {

        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {

        }
    }

    class BuildingState : IState<AntBoid>
    {
        GridManager.GridCellKey m_currentCellKey;

        void IState<AntBoid>.Enter(AntBoid owner)
        {
            m_currentCellKey = GameManager.instance.gridManager.GetCellKey(owner.m_navTarget);
            GameManager.instance.gridManager.AddGridCellObject(m_currentCellKey, owner);
            owner.EnterBuilding();
        }

        void IState<AntBoid>.Exit(AntBoid owner)
        {

        }

        void IState<AntBoid>.Invoke(AntBoid owner)
        {
            GameManager.instance.gridManager.RemoveGridCellObject(m_currentCellKey);
        }
    }

    #endregion // !States
}
