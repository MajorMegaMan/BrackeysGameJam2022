using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimationManager : MonoBehaviour
{
    [SerializeField] MenuAnimationGroup m_mainMenu = null;
    [SerializeField] MenuAnimationGroup m_settingsPanel = null;
    [SerializeField] MenuAnimationGroup m_creditsPanel = null;

    [SerializeField] ExtendedPixelController m_pixelController = null;
    [SerializeField] float m_fadeInTime = 1.0f;
    [SerializeField] float m_beginPixelRatio = 200.0f;

    [Header("Camera")]
    [SerializeField] SimpleFollow m_cameraFollow = null;
    [SerializeField] Transform m_camSpinner = null;
    [SerializeField] Transform m_camSpinnerLookTarget = null;
    [SerializeField] float m_spinMoveTime = 0.1f;
    [SerializeField] float m_spinLookTime = 0.1f;

    [SerializeField] Transform m_creditsPos = null;
    [SerializeField] Transform m_creditsLookTarget = null;
    [SerializeField] float m_creditsMoveTime = 1.0f;
    [SerializeField] float m_creditsLookTime = 1.0f;

    delegate void UpdateDelegate();
    UpdateDelegate m_updateDelegate;

    float m_updateTimer = 0.0f;

    float m_targetPixelRatio;

    private void Awake()
    {
        m_updateDelegate = UpdateFade;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_targetPixelRatio = m_pixelController.pixelRatio;
        m_pixelController.pixelRatio = m_beginPixelRatio;
        m_pixelController.UpdateLastCamSize();

        SetCameraSpinning();
    }

    // Update is called once per frame
    void Update()
    {
        m_updateDelegate.Invoke();
    }

    void UpdateFade()
    {
        m_updateTimer += Time.deltaTime;
        if(m_updateTimer > m_fadeInTime)
        {
            m_updateTimer -= m_fadeInTime;
            m_updateDelegate = Empty;

            m_pixelController.pixelRatio = Mathf.Lerp(m_beginPixelRatio, m_targetPixelRatio, 1.0f);

            m_mainMenu.BeginEnter();
        }
        else
        {
            m_pixelController.pixelRatio = Mathf.Lerp(m_beginPixelRatio, m_targetPixelRatio, m_updateTimer / m_fadeInTime);
        }
    }

    void Empty()
    {

    }

    public void ShowMain()
    {
        m_mainMenu.BeginEnter();
        m_settingsPanel.BeginExit();
        m_creditsPanel.BeginExit();
    }

    public void ShowSettings()
    {
        m_mainMenu.BeginExit();
        m_settingsPanel.BeginEnter();
    }

    public void ShowCredits()
    {
        m_mainMenu.BeginExit();
        m_settingsPanel.BeginExit();
        m_creditsPanel.BeginEnter();
    }

    public void SetCameraCredits()
    {
        m_cameraFollow.SetTargetFollow(m_creditsPos);
        m_cameraFollow.SetLookTarget(m_creditsLookTarget);

        m_cameraFollow.smoothPosTime = m_creditsMoveTime;
        m_cameraFollow.smoothLookTime = m_creditsLookTime;
    }

    public void SetCameraSpinning()
    {
        m_cameraFollow.SetTargetFollow(m_camSpinner);
        m_cameraFollow.SetLookTarget(m_camSpinnerLookTarget);

        m_cameraFollow.smoothPosTime = m_spinMoveTime;
        m_cameraFollow.smoothLookTime = m_spinLookTime;
    }
}
