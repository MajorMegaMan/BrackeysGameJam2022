using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    [SerializeField] AntSettings m_settings = null;
    public PlayerController m_player = null;

    List<AntBoid> m_playerGroupedAnts = null;
    List<AntBoid> m_playerBuildingAnts = null;

    public AntSettings settings { get { return m_settings; } }
    public PlayerController player { get { return m_player; } }

    // Singletons baby.
    static AntManager _instance = null;
    public static AntManager instance { get { return _instance; } }

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
        }
        _instance = this;

        m_playerGroupedAnts = new List<AntBoid>();
        m_playerBuildingAnts = new List<AntBoid>();
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
        
    }

    public void DebugClearBuildAnts()
    {
        while(m_playerBuildingAnts.Count != 0)
        {
            m_playerBuildingAnts[0].PlayerDropTriggerEvent();
        }
    }

    public AntBoid GetAvailableAnt()
    {
        if(m_playerGroupedAnts.Count > 0)
        {
            int i = Random.Range(0, m_playerGroupedAnts.Count);
            return m_playerGroupedAnts[i];
        }
        else
        {
            return null;
        }
    }

    #region AntListManagement
    public void AddToPlayerGroup(AntBoid antBoid)
    {
        m_playerGroupedAnts.Add(antBoid);
    }

    public void RemoveFromPlayerGroup(AntBoid antBoid)
    {
        m_playerGroupedAnts.Remove(antBoid);
    }

    public void AddToBuildAnts(AntBoid antBoid)
    {
        m_playerBuildingAnts.Add(antBoid);
    }

    public void RemoveFromBuildAnts(AntBoid antBoid)
    {
        m_playerBuildingAnts.Remove(antBoid);
    }
    #endregion // !AntListManagement
}
