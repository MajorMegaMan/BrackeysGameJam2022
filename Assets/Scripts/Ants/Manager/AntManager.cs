using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AntManager : MonoBehaviour
{
    [SerializeField] AntSettings m_settings = null;

    List<AntBoid> m_allAnts = null;

    List<AntBoid> m_playerGroupedAnts = null;
    List<AntBuildingGroup> m_buildGroups = null;

    [SerializeField] BridgeCollider m_bridgeColliderPrefab = null;

    // UI
    [SerializeField] TMP_Text m_UIantCountText;

    public AntSettings settings { get { return m_settings; } }
    public PlayerController player { get { return GameManager.instance.player; } }

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
        m_UIantCountText.text = m_playerGroupedAnts.Count.ToString();
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
        while (m_buildGroups.Count > 0)
        {
            var group = m_buildGroups[0];
            group.Dissassemble();
        }
    }

    // called during ant build group disassemble
    public void RemoveBuildGroup(AntBuildingGroup buildGroup)
    {
        m_buildGroups.Remove(buildGroup);
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

    public bool InitiateLineBuild(Vector3 start, Vector3 end)
    {
        float lineLength = (end - start).magnitude;
        int antCount = CalculateLineAntCount(lineLength);

        if(CheckLineRay(start, end, lineLength))
        {
            return false;
        }

        if (antCount <= AvailableAntCount())
        {
            BridgeCollider newBridge = Instantiate(m_bridgeColliderPrefab, start, Quaternion.identity, transform);
            newBridge.SetBridgeLine(start, end);

            AntBuildingGroup newGroup = new AntBuildingGroup(start, end, antCount, newBridge);
            m_buildGroups.Add(newGroup);
            for (int i = 0; i < antCount; i++)
            {
                AntBoid antBoid = GetAvailableAnt();
                antBoid.SetBuildGroup(newGroup);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckLineRay(Vector3 start, Vector3 end, float lineLength)
    {
        return Physics.Raycast(start, (end - start), lineLength, settings.environmentObstuctionLayerMask, QueryTriggerInteraction.Ignore);
    }

    public bool CheckLineRay(Vector3 start, Vector3 end)
    {
        Vector3 toEnd = (end - start);
        return Physics.Raycast(start, toEnd, toEnd.magnitude, settings.environmentObstuctionLayerMask, QueryTriggerInteraction.Ignore);
    }

    public int CalculateLineAntCount(Vector3 start, Vector3 end)
    {
        float lineLength = (end - start).magnitude;
        return CalculateLineAntCount(lineLength);
    }

    public int CalculateLineAntCount(float lineMagnitude)
    {
        lineMagnitude /= settings.singleBuildHeight;
        return (int)Mathf.Ceil(lineMagnitude);
    }

    public bool SendGroupToCarryObject(CarryableObject carryableObject, Vector3 targetCarryPosition, Quaternion targetCarryRotation)
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
            return true;
        }
        return false;
    }

    #region AntListManagement
    public void AddToPlayerGroup(AntBoid antBoid)
    {
        m_playerGroupedAnts.Add(antBoid);
        m_UIantCountText.text = m_playerGroupedAnts.Count.ToString();
    }

    public void RemoveFromPlayerGroup(AntBoid antBoid)
    {
        m_playerGroupedAnts.Remove(antBoid);
        m_UIantCountText.text = m_playerGroupedAnts.Count.ToString();
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
