using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerController m_player = null;
    [SerializeField] PlayerAudio m_playerAudio = null;
    [SerializeField] PlayerAnimate m_playerAnimate = null;
    [SerializeField] GridManager m_gridManager = null;

    [SerializeField] AudioClip m_defaultMusicClip = null;
    [SerializeField] float m_defaultMusicVolume = 0.15f;
    [SerializeField] AudioMixerGroup m_defaultMusicMixer = null;

    [SerializeField] MenuAnimationGroup m_pauseMenu = null;
    [SerializeField] MenuAnimationGroup m_settingsPanel = null;
    [SerializeField] MenuAnimationGroup m_winPanel = null;
    bool m_gamePaused = false;

    PackagedStateMachine<GameManager> m_selectionStateMachine;

    // CameraRelated
    [Header("Camera Var")]
    [SerializeField] SimpleFollow m_cameraFollow = null;
    [SerializeField] CamLookTarget m_camLookTarget = null;
    [SerializeField] float m_regularCamLerp = 0.1f;
    [SerializeField] float m_drawingCamLerp = 0.5f;

    [SerializeField] MouseIndicator m_mouseIndicator = null;

    // Selection StateLogic
    // Carry Hover
    CarryableObject m_carryableObject = null;
    Collider m_hoverCollider = null;

    [Header("Selection")]
    [SerializeField] LayerMask m_carryPlacementInvalidLayer = 0;

    // This will shrink the size of the mouse indicator whne performing collider checks
    [SerializeField] float m_boxShrinkDetector = 0.9f;

    NavMeshPath m_startPath;
    NavMeshPath m_endPath;

    [Header("Win Game")]
    [SerializeField] float m_winSpinSpeed = 1.0f;

    // Singletons baby.
    static GameManager _instance = null;
    public static GameManager instance { get { return _instance; } }

    // Getters
    public GridManager gridManager { get { return m_gridManager; } }
    public PlayerController player { get { return m_player; } }
    public PlayerAudio playerAudio { get { return m_playerAudio; } }
    public AudioClip defaultMusicClip { get { return m_defaultMusicClip; } }
    public float defaultMusicVolume { get { return m_defaultMusicVolume; } }
    public AudioMixerGroup defaultMusicMixer { get { return m_defaultMusicMixer; } }

    public bool gamePaused { get { return m_gamePaused; } }

    bool m_hasWon = false;
    public bool hasWon { get { return m_hasWon; } }

    float m_winTimer = 0.0f;

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

        m_cameraFollow.SetLookTarget(m_camLookTarget.transform);
        m_cameraFollow.SetTargetFollow(player.transform);

        m_camLookTarget.SetFollowTarget(player.ragdoll.rigidBodies[0].transform);
        m_camLookTarget.SetMode(false);
        m_camLookTarget.splitLerpAmount = m_regularCamLerp;

        m_mouseIndicator.EnableText(false);

        m_startPath = new NavMeshPath();
        m_endPath = new NavMeshPath();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_mouseIndicator.FinishDrawLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (m_gamePaused)
            {
                m_pauseMenu.BeginExit();
                m_settingsPanel.BeginExit();
                m_gamePaused = false;
            }
            else
            {
                m_pauseMenu.BeginEnter();
                m_gamePaused = true;
            }
        }

        if (hasWon == true)
        {
            m_winTimer += Time.deltaTime * Random.Range(0.5f, 1.0f);
            if(m_winTimer > 5.0f)
            {
                m_winTimer -= 5.0f;
                playerAudio.PlayPositiveAntSound();
            }
            return;
        }

        if(!gamePaused)
        {
            m_mouseIndicator.CamRayCast();
            m_selectionStateMachine.Invoke();
        }
    }

    public bool InitiateAntBuild(Vector3 start, Vector3 end)
    {
        bool validStartPath = NavMesh.SamplePosition(start, out NavMeshHit startNavHit, 2.0f, ~0);
        if (validStartPath)
        {
            validStartPath = AntManager.instance.IsValidNavPath(player.transform.position, startNavHit.position, m_startPath);
        }

        bool validEndPath = NavMesh.SamplePosition(end, out NavMeshHit endNavHit, 2.0f, ~0);
        if (validEndPath)
        {
            validEndPath = AntManager.instance.IsValidNavPath(player.transform.position, endNavHit.position, m_endPath);
        }

        if(!validEndPath && !validStartPath)
        {
            return false;
        }

        return AntManager.instance.InitiateLineBuild(start, end, m_startPath, m_endPath);
    }

    public void UpdateSelectorColour(int antCount, bool valid = true)
    {
        m_mouseIndicator.SetSelectionColour(valid && (antCount <= AntManager.instance.AvailableAntCount() && antCount != 0));
    }

    public void WinGame()
    {
        m_hasWon = true;
        playerAudio.PlayPositiveAntSound();
        m_playerAnimate.Dance();
        m_winPanel.BeginEnter();
        var camSpin = m_cameraFollow.GetComponent<CameraSpin>();
        if(camSpin != null)
        {
            camSpin.SetVelocity(Vector3.right * m_winSpinSpeed);
        }
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

    void InvokeEmpty()
    {
        if (m_mouseIndicator.IsLastHitColliderCarriable())
        {
            ChangeToState(SelectionStateEnum.hoverCarryObject);
            return;
        }

        if (m_mouseIndicator.IsLastHitColliderBridge())
        {
            ChangeToState(SelectionStateEnum.hoverBridge);
            return;
        }

        // Find selection colour. for now just set to valid
        bool validPath = NavMesh.SamplePosition(m_mouseIndicator.lastCellPosition, out NavMeshHit navHit, 2.0f, ~0);
        if(validPath)
        {
            validPath = AntManager.instance.IsValidNavPath(player.transform.position, navHit.position, m_startPath);
            validPath = m_startPath.status == NavMeshPathStatus.PathComplete;
        }

        m_mouseIndicator.SetSelectionColour(validPath);

        if (Input.GetMouseButtonDown(0))
        {
            if(validPath)
            {
                EmptyStateClick();
            }
            else
            {
                playerAudio.PlayNegativeAntSound();
            }
        }
    }

    void EnterDrawing()
    {
        m_mouseIndicator.StartDrawLine();

        m_camLookTarget.splitLerpAmount = m_drawingCamLerp;

        m_mouseIndicator.EnableText(true);
    }

    void ExitDrawing()
    {
        m_mouseIndicator.FinishDrawLine();

        // Send ants to build with this line
        if (InitiateAntBuild(m_mouseIndicator.GetLineStart(), m_mouseIndicator.GetLineEnd()))
        {
            playerAudio.PlayPositiveAntSound();
        }
        else
        {
            playerAudio.PlayNegativeAntSound();
        }

        //m_camLookTarget.SetMode(true);
        m_camLookTarget.splitLerpAmount = m_regularCamLerp;

        m_mouseIndicator.EnableText(false);
    }

    void InvokeDrawLine()
    {
        m_mouseIndicator.SetEndDrawLine();
        bool validLineRay = !AntManager.instance.CheckLineRay(m_mouseIndicator.GetLineStart(), m_mouseIndicator.GetLineEnd());

        if (Input.GetMouseButtonUp(0))
        {
            ChangeToState(SelectionStateEnum.empty);
            return;
        }

        int antCount = AntManager.instance.CalculateLineAntCount(m_mouseIndicator.GetLineStart(), m_mouseIndicator.GetLineEnd());
        UpdateSelectorColour(antCount, validLineRay);

        m_mouseIndicator.UpdateTextForLine(Camera.main.transform, antCount);
    }

    void EnterHoverCarry()
    {
        m_hoverCollider = m_mouseIndicator.lastHitCollider;
        m_carryableObject = m_hoverCollider.GetComponent<CarryableObject>();

        m_mouseIndicator.EnableText(true);
    }

    void ExitHoverCarry()
    {
        if(m_selectionStateMachine.GetCurrentState() != SelectionStateEnum.drawCarryObject)
        {
            m_mouseIndicator.SetSelectorScaleToGridCellSize();
            m_hoverCollider = null;
            m_carryableObject = null;

            m_mouseIndicator.EnableText(false);
        }
    }

    void InvokeHoverCarry()
    {
        if (m_mouseIndicator.lastHitCollider != m_hoverCollider)
        {
            ChangeToState(SelectionStateEnum.empty);
            return;
        }

        m_mouseIndicator.CoverLastHitCarriableCollider();

        UpdateSelectorColour(m_carryableObject.antStrengthCount);

        m_mouseIndicator.UpdateTextForCarryObject(Camera.main.transform, m_carryableObject);

        if (Input.GetMouseButtonDown(0))
        {
            ChangeToState(SelectionStateEnum.drawCarryObject);
        }
    }

    void EnterDrawCarry()
    {
        m_mouseIndicator.SetSelectorScale(m_hoverCollider.bounds.size);

        m_mouseIndicator.StartDrawLine();
        m_camLookTarget.splitLerpAmount = m_drawingCamLerp;
    }

    void ExitDrawCarry()
    {
        m_mouseIndicator.SetSelectorScaleToGridCellSize();
        m_hoverCollider = null;
        m_carryableObject = null;

        m_mouseIndicator.EnableText(false);

        m_mouseIndicator.FinishDrawLine();
        m_camLookTarget.splitLerpAmount = m_regularCamLerp;
    }

    Collider[] FindMouseColliders()
    {
        return Physics.OverlapBox(m_mouseIndicator.transform.position, m_mouseIndicator.transform.localScale * m_boxShrinkDetector * 0.5f, m_mouseIndicator.transform.rotation, m_carryPlacementInvalidLayer, QueryTriggerInteraction.Ignore);
    }

    void InvokeDrawCarry()
    {
        m_mouseIndicator.SetEndDrawLine();

        m_mouseIndicator.UpdateTextForCarryObject(Camera.main.transform, m_carryableObject);

        var colliders = FindMouseColliders();
        bool isBlocked = colliders.Length > 0;

        m_mouseIndicator.SetSelectionColour(!isBlocked);

        if (Input.GetMouseButtonUp(0))
        {
            // Try to apply ant object placement.
            if(isBlocked)
            {
                for(int i = 0; i < colliders.Length; i++)
                {
                    Debug.Log(colliders[i].name);
                }
                playerAudio.PlayNegativeAntSound();
                ChangeToState(SelectionStateEnum.empty);
                return;
            }

            if(AntManager.instance.SendGroupToCarryObject(m_carryableObject, m_mouseIndicator.transform.position, m_mouseIndicator.transform.rotation))
            {
                playerAudio.PlayPositiveAntSound();
            }
            else
            {
                playerAudio.PlayNegativeAntSound();
            }

            ChangeToState(SelectionStateEnum.empty);
        }
    }

    void EnterHoverBridge()
    {
        m_hoverCollider = m_mouseIndicator.lastHitCollider;

        m_mouseIndicator.SetSelectionColour(MouseIndicator.SelectionColour.bridge);

        m_mouseIndicator.CoverLastHitBridgeCollider();
    }

    void ExitHoverBridge()
    {
        m_mouseIndicator.SetSelectorScaleToGridCellSize();
        m_mouseIndicator.transform.rotation = Quaternion.identity;
    }

    void InvokeHoverBridge()
    {
        if (m_mouseIndicator.lastHitCollider != m_hoverCollider)
        {
            ChangeToState(SelectionStateEnum.empty);
            return;
        }

        m_mouseIndicator.CoverLastHitBridgeCollider();

        if (Input.GetMouseButtonDown(0))
        {
            var bridge = m_hoverCollider.GetComponent<BridgeCollider>();
            bridge.Disassemble();
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
        states[(int)SelectionStateEnum.hoverBridge] = new HoverBridgeState();

        m_selectionStateMachine = new PackagedStateMachine<GameManager>(this, states);

        m_selectionStateMachine.InitialiseState(SelectionStateEnum.empty);
    }

    public enum SelectionStateEnum
    {
        empty,
        drawing,
        hoverCarryObject,
        drawCarryObject,
        hoverBridge
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
            owner.InvokeEmpty();
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
            owner.InvokeDrawLine();
        }
    }

    class HoverCarryObjectState : IState<GameManager>
    {
        void IState<GameManager>.Enter(GameManager owner)
        {
            owner.EnterHoverCarry();
        }

        void IState<GameManager>.Exit(GameManager owner)
        {
            owner.ExitHoverCarry();
        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            owner.InvokeHoverCarry();
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

    class HoverBridgeState : IState<GameManager>
    {
        void IState<GameManager>.Enter(GameManager owner)
        {
            owner.EnterHoverBridge();
        }

        void IState<GameManager>.Exit(GameManager owner)
        {
            owner.ExitHoverBridge();
        }

        void IState<GameManager>.Invoke(GameManager owner)
        {
            owner.InvokeHoverBridge();
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
