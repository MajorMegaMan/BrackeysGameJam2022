using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarryableObject : MonoBehaviour
{
    [SerializeField] int m_antStrengthCount = 4;

    [SerializeField] Collider m_spaceCollider = null;

    [SerializeField] float m_additionalRot = 0.0f;

    [SerializeField] float m_antDistanceOffset = 0.5f;
    [SerializeField] float m_antHeightOffset = -0.5f;
    [SerializeField] float m_directionSearchMultiplier = 5.0f;

    List<AntBoid> m_currentAntGroup;
    List<AntBoid> m_arrivedAnts;

    [SerializeField] NavMeshObstacle m_navObstacle;
    [SerializeField] NavMeshAgent m_navAgent;
    [SerializeField] float m_arriveSnapDistance = 0.01f;

    Vector3 m_targetWalkPos = Vector3.zero;
    Quaternion m_targetRotation = Quaternion.identity;

    public int antStrengthCount { get { return m_antStrengthCount; } }

    // navagent variable getters
    public float speed { get { return m_navAgent.speed; } }
    public Vector3 velocity { get { return m_navAgent.velocity; } }
    public float currentSpeed { get { return m_navAgent.velocity.magnitude; } }

    private void Awake()
    {
        m_currentAntGroup = new List<AntBoid>(m_antStrengthCount);
        m_arrivedAnts = new List<AntBoid>(m_antStrengthCount);

        m_navObstacle.enabled = true;
        m_navAgent.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_navAgent.enabled)
        {
            if(m_navAgent.pathPending)
            {
                return;
            }

            if(m_navAgent.remainingDistance < m_arriveSnapDistance)
            {
                m_navObstacle.enabled = true;
                m_navAgent.enabled = false;
                transform.position = m_targetWalkPos;
                transform.rotation = m_targetRotation;

                while(m_currentAntGroup.Count != 0)
                {
                    // Setting the ants to wait should remove them from the ant group while also correctly removing references on their side.
                    m_currentAntGroup[0].SetToWait();
                }
            }
        }
    }

    Vector3 CalculateMountingDirection(int positionCount)
    {
        float degree = (360.0f / m_antStrengthCount) * positionCount;
        degree += m_additionalRot;
        Quaternion rot = Quaternion.AngleAxis(degree, Vector3.up);

        return rot * Vector3.forward;
    }

    public Vector3 GetAntPosition(int positionCount)
    {
        Vector3 dir = CalculateMountingDirection(positionCount);
        Vector3 closestPoint = m_spaceCollider.ClosestPoint(transform.position + dir * m_directionSearchMultiplier);
        Vector3 surroundingPoint = closestPoint + dir * m_antDistanceOffset + Vector3.up * m_antHeightOffset;

        Vector3 result = surroundingPoint;
        if(NavMesh.SamplePosition(surroundingPoint, out NavMeshHit navMeshHit, 2.0f, ~0))
        {
            result = navMeshHit.position;
        }
        return result;
    }

    public Vector3 GetAntCentre()
    {
        return transform.position + Vector3.up * m_antHeightOffset;
    }

    public void AddAnt(AntBoid ant)
    {
        m_currentAntGroup.Add(ant);
    }

    public void RemoveAnt(AntBoid ant)
    {
        m_currentAntGroup.Remove(ant);
        m_arrivedAnts.Remove(ant);
    }

    public void AntArrived(AntBoid ant)
    {
        m_arrivedAnts.Add(ant);

        if (m_arrivedAnts.Count >= m_antStrengthCount)
        {
            m_navObstacle.enabled = false;
            m_navAgent.enabled = true;
            m_navAgent.SetDestination(m_targetWalkPos);
        }
    }

    public void MoveToLocation(Vector3 position, Quaternion rotation)
    {
        m_targetWalkPos = position;
        m_targetRotation = rotation;
    }

    private void OnDrawGizmos()
    {
        if(m_spaceCollider != null)
        {
            for (int i = 0; i < m_antStrengthCount; i++)
            {
                Vector3 antPos = GetAntPosition(i);
                Gizmos.DrawSphere(antPos, 0.2f);
            }
        }
    }
}
