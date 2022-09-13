using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BridgeCollider : MonoBehaviour
{
    [SerializeField] CapsuleCollider m_capsuleCollider = null;
    [SerializeField] OffMeshLink m_offMeshLink = null;
    [SerializeField] Transform m_navStart = null;
    [SerializeField] Transform m_navEnd = null;

    AntBuildingGroup m_ownerGroup = null;

    private void Start()
    {
        EnableNavLink(false);
    }

    public void SetBridgeLine(Vector3 start, Vector3 end)
    {
        SetRotation(end - start);
        SetNavPositions(start, end);
    }

    void SetRotation(Vector3 lineDir)
    {
        transform.up = lineDir;
    }

    public void EnableNavLink(bool enabled)
    {
        m_offMeshLink.activated = enabled;
    }

    void SetNavPositions(Vector3 start, Vector3 end)
    {
        if(NavMesh.SamplePosition(start, out NavMeshHit startHit, 2, ~0))
        {
            m_navStart.position = startHit.position;
        }
        else
        {
            m_navStart.position = start;
        }

        if (NavMesh.SamplePosition(end, out NavMeshHit endHit, 2, ~0))
        {
            m_navEnd.position = endHit.position;
        }
        else
        {
            m_navEnd.position = end;
        }
    }

    public void SetLength(float length)
    {
        m_capsuleCollider.height = length;// + m_capsuleCollider.radius * 2;
        Vector3 center = m_capsuleCollider.center;
        center.y = m_capsuleCollider.height * 0.5f - GameManager.instance.gridManager.cellSize * 0.5f;
        m_capsuleCollider.center = center;
    }

    public void DestroyBridge()
    {
        // For now just destroy the object
        Destroy(gameObject);
    }

    public void SetBuildingGroup(AntBuildingGroup antBuildingGroup)
    {
        m_ownerGroup = antBuildingGroup;
    }

    public void Disassemble()
    {
        m_ownerGroup.Dissassemble();
    }
}
