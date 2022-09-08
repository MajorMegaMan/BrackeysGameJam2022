using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntBuildingGroup
{
    List<AntBoid> m_allAnts = new List<AntBoid>();
    Queue<AntBoid> m_waitingToBuildQueue = new Queue<AntBoid>();
    List<AntBoid> m_frozenList = new List<AntBoid>();

    Vector3 m_start = Vector3.zero;
    Vector3 m_end = Vector3.zero;

    int m_antLineCount = 0;

    BridgeCollider m_bridgeCollider = null;

    public Vector3 start { get { return m_start; } }
    public Vector3 end { get { return m_end; } }

    bool m_startedClimbing = false;

    public AntBuildingGroup(Vector3 start, Vector3 end, int antLinecount, BridgeCollider bridgeCollider)
    {
        m_start = start;
        m_end = end;
        m_antLineCount = antLinecount;

        m_bridgeCollider = bridgeCollider;
        m_bridgeCollider.gameObject.SetActive(false);
        m_bridgeCollider.SetBuildingGroup(this);
    }

    public void AddAntToGroup(AntBoid antBoid)
    {
        m_allAnts.Add(antBoid);
    }

    public void AddToWaitingGroup(AntBoid antBoid)
    {
        m_waitingToBuildQueue.Enqueue(antBoid);
        if(!m_startedClimbing)
        {
            SendNextAntToClimbPosition();
        }
    }

    // Should use ants from the waiting queue. Otherwise unexpected behaviour might occur.
    public void FreezeAnt(AntBoid antBoid)
    {
        m_bridgeCollider.gameObject.SetActive(true);

        m_frozenList.Add(antBoid);
        m_startedClimbing = false;
        SendNextAntToClimbPosition();
        m_bridgeCollider.SetLength((CalculateClimbPosition() - start).magnitude);

        if(m_frozenList.Count >= m_antLineCount)
        {
            m_bridgeCollider.EnableNavLink(true);
        }
    }

    public Vector3 GetLine()
    {
        return end - start;
    }

    public Vector3 CalculateClimbPosition()
    {
        Vector3 linePos = Vector3.LerpUnclamped(start, end, (m_frozenList.Count + 0.5f) / (float)m_antLineCount);

        Vector3 offsetDir = Vector3.Cross(GetLine(), Vector3.up);
        offsetDir = Vector3.Cross(GetLine(), offsetDir);

        return linePos + offsetDir.normalized * AntManager.instance.settings.bridgeOffsetDistance;
    }

    public void SendNextAntToClimbPosition()
    {
        if (m_waitingToBuildQueue.Count > 0)
        {
            m_startedClimbing = true;
            var ant = m_waitingToBuildQueue.Dequeue();
            ant.StartClimbing(CalculateClimbPosition());
        }
    }

    public void Dissassemble()
    {
        for(int i = 0; i < m_allAnts.Count; i++)
        {
            m_allAnts[i].SetBuildGroup(null);
        }

        m_waitingToBuildQueue.Clear();
        m_frozenList.Clear();

        m_bridgeCollider.DestroyBridge();

        AntManager.instance.RemoveBuildGroup(this);
    }
}
