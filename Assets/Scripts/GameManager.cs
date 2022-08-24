using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager = null;

    [SerializeField] int debugLineAntCount = 0;

    PackagedStateMachine<GameManager> m_selectionStateMachine;

    // CameraRelated
    [SerializeField] CamLookTarget m_camLookTarget = null;
    [SerializeField] float m_regularCamLerp = 0.1f;
    [SerializeField] float m_drawingCamLerp = 0.5f;

    [SerializeField] MouseIndicator m_mouseIndicator = null;

    // Selection StateLogic
    // Carry Hover
    CarryableObject m_carryableObject = null;
    Collider m_hoverCollider = null;

    // Singletons baby.
    static GameManager _instance = null;
    public static GameManager instance { get { return _instance; } }

    // Getters
    public GridManager gridManager { get { return m_gridManager; } }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;

        Initialise();
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    void Initialise()
    {
        InitialiseStateMachine();
        m_camLookTarget.SetMode(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mouseIndicator.FinishDrawLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AntManager.instance.ClearBuildGroups();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            AntManager.instance.ReleaseAllAnts();
        }

        if (m_mouseIndicator.CamRayCast())
        {
            m_selectionStateMachine.Invoke();
        }
    }

    public void InitiateAntBuild(Vector3 start, Vector3 end)
    {
        AntManager.instance.InitiateLineBuild(start, end);
    }

    #region SelectionStates

    void ChangeToState(SelectionStateEnum selectionState)
    {
        m_selectionStateMachine.ChangeToState(selectionState);
    }

    #region SelectionStateCallables
    void EmptyStateClick()
    {
        ChangeToState(SelectionStateEnum.drawing);
    }

    void EnterDrawing()
    {
        m_mouseIndicator.StartDrawLine();

        m_camLookTarget.splitLerpAmount = m_drawingCamLerp;
    }

    void ExitDrawing()
    {
        m_mouseIndicator.FinishDrawLine();

        // Send ants to build with this line
        InitiateAntBuild(m_mouseIndicator.GetLineStart(), m_mouseIndicator.GetLineEnd());

        //m_camLookTarget.SetMode(true);
        m_camLookTarget.splitLerpAmount = m_regularCamLerp;
    }

    void InvokeDrawLine()
    {
        m_mouseIndicator.SetEndDrawLine();

        debugLineAntCount = AntManager.instance.CalculateLineAntCount(m_mouseIndicator.GetLineStart(), m_mouseIndicator.GetLineEnd());
    }

    void EnterHover()
    {
        m_hoverCollider = m_mouseIndicator.lastHitCollider;
        m_carryableObject = m_hoverCollider.GetComponent<CarryableObject>();
    }

    void ExitHover()
    {
        if(m_selectionStateMachine.GetCurrentState() != SelectionStateEnum.drawCarryObject)
        {
            m_mouseIndicator.SetSelectorScaleToGridCellSize();
            m_hoverCollider = null;
            m_carryableObject = null;
        }
    }

    void InvokeHover()
    {
        if (m_mouseIndicator.lastHitCollider != m_hoverCollider)
        {
            ChangeToState(SelectionStateEnum.empty);
            return;
        }

        m_mouseIndicator.CoverLastHitCollider();

        if (Input.GetMouseButtonDown(0))
        {
            ChangeToState(SelectionStateEnum.drawCarryObject);
        }
    }

    void EnterDrawCarry()
    {
        m_mouseIndicator.SetSelectorScale(m_hoverCollider.bounds.size);

        m_mouseIndicator.StartDrawLine();
    }

    void ExitDrawCarry()
    {
        m_mouseIndicator.SetSelectorScaleToGridCellSize();
        m_hoverCollider = null;
        m_carryableObject = null;

        m_mouseIndicator.FinishDrawLine();
    }

    void InvokeDrawCarry()
    {
        m_mouseIndicator.SetEndDrawLine();

        if (Input.GetMouseButtonUp(0))
        {
            // Try to apply ant object placement.
            AntManager.instance.SendGroupToCarryObject(m_carryableObject, m_mouseIndicator.transform.position, m_mouseIndicator.transform.rotation);

            ChangeToState(SelectionStateEnum.empty);
        }
    }

    #endregion

    void InitialiseStateMachine()
    {
        var enumValues = System.Enum.GetValues(typeof(SelectionStateEnum));
        IState<GameManager>[] states = new IState<GameManager>[enumValues.Length];

        states[(int)SelectionStateEnum.empty] = new EmptyState();
        states[(int)SelectionStateEnum.drawing] = new DrawingState();
        states[(int)SelectionStateEnum.hoverCarryObject] = new HoverCarryObjectState();
        states[(int)SelectionStateEnum.drawCarryObject] = new DrawCarryObjectState();

        m_selectionStateMachine = new PackagedStateMachine<GameManager>(this, states);

        m_selectionStateMachine.InitialiseState(SelectionStateEnum.empty);
    }

    public enum SelectionStateEnum
    {
        empty,
        drawing,
        hoverCarryObject,
        drawCarryObject
    }

    class EmptyState : IState<GameManager>
    {
        void IState<GameManager>.Enter(GameManager owner)
        {
            
        }

        void IState<GameManager>.Exit(GameManager owner)
        {

        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            if (owner.m_mouseIndicator.IsLastHitColliderCarriable())
            {
                owner.ChangeToState(SelectionStateEnum.hoverCarryObject);
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                owner.EmptyStateClick();
            }
        }
    }

    class DrawingState : IState<GameManager>
    {
        void IState<GameManager>.Enter(GameManager owner)
        {
            owner.EnterDrawing();
        }

        void IState<GameManager>.Exit(GameManager owner)
        {
            owner.ExitDrawing();
        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            if (Input.GetMouseButtonUp(0))
            {
                owner.ChangeToState(SelectionStateEnum.empty);
                return;
            }

            owner.InvokeDrawLine();
        }
    }

    class HoverCarryObjectState : IState<GameManager>
    {
        void IState<GameManager>.Enter(GameManager owner)
        {
            owner.EnterHover();
        }

        void IState<GameManager>.Exit(GameManager owner)
        {
            owner.ExitHover();
        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            owner.InvokeHover();
        }
    }

    class DrawCarryObjectState : IState<GameManager>
    {
        void IState<GameManager>.Enter(GameManager owner)
        {
            owner.EnterDrawCarry();
        }

        void IState<GameManager>.Exit(GameManager owner)
        {
            owner.ExitDrawCarry();
        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            owner.InvokeDrawCarry();
        }
    }
    #endregion // !SelectionStates
}

public static class PackagedSMExtension
{
    public static void InitialiseState(this PackagedStateMachine<GameManager> packagedStateMachine, GameManager.SelectionStateEnum selectionState)
    {
        packagedStateMachine.InitialiseState((int)selectionState);
    }

    public static void ChangeToState(this PackagedStateMachine<GameManager> packagedStateMachine, GameManager.SelectionStateEnum selectionState)
    {
        packagedStateMachine.ChangeToState((int)selectionState);
    }

    public static GameManager.SelectionStateEnum GetCurrentState(this PackagedStateMachine<GameManager> packagedStateMachine)
    {
        return (GameManager.SelectionStateEnum)packagedStateMachine.currentIndex;
    }
}
