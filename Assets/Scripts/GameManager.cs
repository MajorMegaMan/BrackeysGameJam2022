using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager = null;
    [SerializeField] RaySelector m_raySelector = null;
    [SerializeField] BuildLineTool m_lineTool = null;
    bool m_isDrawingLine = false;

    [SerializeField] int debugLineAntCount = 0;

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
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
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
            AntManager.instance.DebugClearBuildAnts();
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (m_raySelector.CamRayCastPoint(out Vector3 hitPoint))
            {
                Vector3 cellPos = m_gridManager.GetCellFloorPosition(m_gridManager.GetCellKey(hitPoint));
                m_lineTool.SetStart(cellPos);
                m_lineTool.ShowLine();
                m_isDrawingLine = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_raySelector.CamRayCastPoint(out Vector3 hitPoint))
            {
                Vector3 cellPos = m_gridManager.GetCellPosition(m_gridManager.GetCellKey(hitPoint));
                m_lineTool.SetEnd(cellPos);

                // Send ants to build with this line
                InitiateAntBuild(m_lineTool.GetStart(), cellPos);
            }
            m_lineTool.HideLine();
            m_isDrawingLine = false;
        }

        if (m_isDrawingLine)
        {
            if (m_raySelector.CamRayCastPoint(out Vector3 hitPoint))
            {
                Vector3 cellPos = m_gridManager.GetCellPosition(m_gridManager.GetCellKey(hitPoint));
                m_lineTool.SetEnd(cellPos);
            }

            debugLineAntCount = AntManager.instance.CalculateLineAntCount(m_lineTool.GetStart(), m_lineTool.GetEnd());
        }
    }

    public void InitiateAntBuild(Vector3 start, Vector3 end)
    {
        AntManager.instance.InitiateLineBuild(start, end);
    }

    public void DebugSendAntToBuild(Vector3 position)
    {
        AntBoid ant = AntManager.instance.GetAvailableAnt();
        if(ant != null)
        {
            var cellKey = m_gridManager.GetCellKey(position);
            if(m_gridManager.TryGetCellObject(cellKey, out GridManager.GridCellObject gridCellObject))
            {
                // Tile is occupied
            }
            else
            {
                // Tile is unoccupied
                ant.SendToBuildCell(cellKey);
            }
        }
    }
}
