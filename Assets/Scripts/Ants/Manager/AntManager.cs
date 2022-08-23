using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntManager : MonoBehaviour
{
    [SerializeField] AntSettings m_settings = null;
    public PlayerController m_player = null;

    List<AntBoid> m_playerGroupedAnts = null;

    List<AntBuildingGroup> m_buildGroups = null;

    List<AntBoid> m_allAnts = null;

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

        m_allAnts = new List<AntBoid>();
        m_playerGroupedAnts = new List<AntBoid>();
        m_buildGroups = new List<AntBuildingGroup>();
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

    public void ReleaseAllAnts()
    {
        for(int i = 0; i < m_allAnts.Count; i++)
        {
            m_allAnts[i].SetToWait();
        }
    }

    public void ClearBuildGroups()
    {
        for (int i = 0; i < m_buildGroups.Count; i++)
        {
            var group = m_buildGroups[i];
            group.Dissassemble();
        }
        m_buildGroups.Clear();
    }

    public int AvailableAntCount()
    {
        return m_playerGroupedAnts.Count;
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

    public void InitiateLineBuild(Vector3 start, Vector3 end)
    {
        int antCount = CalculateLineAntCount(start, end);
        if (antCount <= AvailableAntCount())
        {
            AntBuildingGroup newGroup = new AntBuildingGroup(start, end, antCount);
            m_buildGroups.Add(newGroup);
            for(int i = 0; i < antCount; i++)
            {
                AntBoid antBoid = GetAvailableAnt();
                antBoid.SetBuildGroup(newGroup);
            }
        }
    }

    public int CalculateLineAntCount(Vector3 start, Vector3 end)
    {
        float lineLength = (end - start).magnitude;
        lineLength /= settings.singleBuildHeight;
        return (int)Mathf.Ceil(lineLength);
    }

    public void SendGroupToCarryObject(CarryableObject carryableObject, Vector3 targetCarryPosition, Quaternion targetCarryRotation)
    {
        int antCount = carryableObject.antStrengthCount;
        if (AvailableAntCount() >= antCount)
        {
            // Can send ants to pick up an object
            carryableObject.MoveToLocation(targetCarryPosition, targetCarryRotation);
            for(int i = 0; i < antCount; i++)
            {
                var ant = GetAvailableAnt();
                ant.CarryObject(carryableObject, i);
            }
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

    public void AddToAllAnts(AntBoid antBoid)
    {
        m_allAnts.Add(antBoid);
    }

    public void RemoveFromAllAnts(AntBoid antBoid)
    {
        m_allAnts.Remove(antBoid);
    }
    #endregion // !AntListManagement

    private void OnDrawGizmos()
    {
        if(m_buildGroups != null)
        {
            for (int i = 0; i < m_buildGroups.Count; i++)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(m_buildGroups[i].start, 0.2f);
                Gizmos.DrawSphere(m_buildGroups[i].end, 0.2f);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(m_buildGroups[i].start, m_buildGroups[i].end);
            }
        }
    }
}
