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

    public Vector3 start { get { return m_start; } }
    public Vector3 end { get { return m_end; } }

    bool m_startedClimbing = false;

    public AntBuildingGroup(Vector3 start, Vector3 end, int antLinecount)
    {
        m_start = start;
        m_end = end;
        m_antLineCount = antLinecount;
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
        m_frozenList.Add(antBoid);
        m_startedClimbing = false;
        SendNextAntToClimbPosition();
    }

    public Vector3 GetLine()
    {
        return end - start;
    }

    public Vector3 CalculateClimbPosition()
    {
        return Vector3.Lerp(start, end, m_frozenList.Count / (float)m_antLineCount);
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
    }
}
