using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager = null;

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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AntManager.instance.DebugClearBuildAnts();
        }
    }

    public void DebugSendAntToBuild(Vector3 position)
    {
        m_gridManager.GetCellKey(position);
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
