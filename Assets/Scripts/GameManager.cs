using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager = null;
    [SerializeField] RaySelector m_raySelector = null;
    [SerializeField] BuildLineTool m_lineTool = null;

    [SerializeField] int debugLineAntCount = 0;

    PackagedStateMachine<GameManager> m_selectionStateMachine;

    // Selection StateLogic
    [SerializeField] Transform m_mouseHighlightObject = null;
    [SerializeField] LayerMask m_carryObjectLayerMask = 0;

    Collider m_lastHitCollider = null;
    Vector3 m_lastHitpoint = Vector3.zero;
    GridManager.GridCellKey m_lastCellKey;
    Vector3 m_lastCellPosition = Vector3.zero;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        m_lineTool.HideLine();
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

        if (CamRayCast())
        {
            m_selectionStateMachine.Invoke();
        }
    }

    public void InitiateAntBuild(Vector3 start, Vector3 end)
    {
        AntManager.instance.InitiateLineBuild(start, end);
    }

    public bool CamRayCast()
    {
        bool hit = m_raySelector.CamRayCast(out Ray camRay, out RaycastHit hitInfo);
        m_lastHitCollider = hitInfo.collider;
        m_lastHitpoint = hitInfo.point - camRay.direction * m_raySelector.raySkin;

        m_lastCellKey = m_gridManager.GetCellKey(m_lastHitpoint);
        m_lastCellPosition = m_gridManager.GetCellPosition(m_lastCellKey);

        Vector3 mouseHighlightPosition = m_lastCellPosition;
        Vector3 scale = m_mouseHighlightObject.localScale;
        Vector3 halfOffset = Vector3.one * m_gridManager.cellSize * 0.5f;

        mouseHighlightPosition.y -= scale.y * 0.5f;
        mouseHighlightPosition.y += scale.y - halfOffset.y;

        if (!IsOdd((int)scale.x))
        {
            mouseHighlightPosition.x += -Mathf.Sign(Vector3.Dot(Camera.main.transform.forward, Vector3.forward)) * halfOffset.x;
        }

        if (!IsOdd((int)scale.z))
        {
            mouseHighlightPosition.z += -Mathf.Sign(Vector3.Dot(Camera.main.transform.right, Vector3.right)) * halfOffset.z;
        }

        m_mouseHighlightObject.position = mouseHighlightPosition;

        return hit;
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }

    void SetSelectorScale(Vector3 scale)
    {
        m_mouseHighlightObject.localScale = scale;
    }

    void SetSelectorScaleToGridCellSize()
    {
        m_mouseHighlightObject.localScale = Vector3.one * m_gridManager.cellSize;
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
        m_mouseHighlightObject.gameObject.SetActive(true);

        //Vector3 cellFloorPos = m_gridManager.GetCellFloorPosition(m_lastCellKey);
        m_lineTool.SetStart(m_lastCellPosition);
        m_lineTool.SetEnd(m_lastCellPosition + Vector3.up * 0.001f);
        m_lineTool.ShowLine();
    }

    void ExitDrawing()
    {
        m_lineTool.SetEnd(m_lastCellPosition);

        // Send ants to build with this line
        InitiateAntBuild(m_lineTool.GetStart(), m_lastCellPosition);

        m_lineTool.HideLine();
    }

    void InvokeDrawLine()
    {
        m_lineTool.SetEnd(m_lastCellPosition);

        debugLineAntCount = AntManager.instance.CalculateLineAntCount(m_lineTool.GetStart(), m_lineTool.GetEnd());
    }

    void EnterHover()
    {
        m_mouseHighlightObject.gameObject.SetActive(false);
        m_hoverCollider = m_lastHitCollider;
        m_carryableObject = m_hoverCollider.GetComponent<CarryableObject>();
    }

    void ExitHover()
    {
        if(m_selectionStateMachine.GetCurrentState() != SelectionStateEnum.drawCarryObject)
        {
            m_hoverCollider = null;
            m_carryableObject = null;
        }
    }

    void InvokeHover()
    {
        if (m_lastHitCollider != m_hoverCollider)
        {
            ChangeToState(SelectionStateEnum.empty);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ChangeToState(SelectionStateEnum.drawCarryObject);
        }
    }

    void EnterDrawCarry()
    {
        m_mouseHighlightObject.gameObject.SetActive(true);
        SetSelectorScale(m_hoverCollider.bounds.size);
    }

    void ExitDrawCarry()
    {
        SetSelectorScaleToGridCellSize();
        m_hoverCollider = null;
        m_carryableObject = null;
    }

    void InvokeDrawCarry()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // Try to apply ant object placement.
            AntManager.instance.SendGroupToCarryObject(m_carryableObject, m_mouseHighlightObject.position, m_mouseHighlightObject.rotation);

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
            owner.m_mouseHighlightObject.gameObject.SetActive(true);
        }

        void IState<GameManager>.Exit(GameManager owner)
        {

        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            if(Utility.ContainsLayer(owner.m_lastHitCollider.gameObject.layer, owner.m_carryObjectLayerMask))
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
