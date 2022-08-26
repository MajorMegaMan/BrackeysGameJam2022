using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildLineTool : MonoBehaviour
{
    [SerializeField] LineRenderer m_lineRenderer;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        Vector3[] positions = new Vector3[2];
        m_lineRenderer.SetPositions(positions);
    }

    public void SetStart(Vector3 position)
    {
        m_lineRenderer.SetPosition(0, position);
    }

    public void SetEnd(Vector3 position)
    {
        m_lineRenderer.SetPosition(1, position);
    }

    public Vector3 GetStart()
    {
        return m_lineRenderer.GetPosition(0);
    }

    public Vector3 GetEnd()
    {
        return m_lineRenderer.GetPosition(1);
    }

    public void GetLine(out Vector3 start, out Vector3 end)
    {
        start = GetStart();
        end = GetEnd();
    }

    public float GetLineLength()
    {
        return (GetEnd() - GetStart()).magnitude;
    }

    public void ShowLine()
    {
        m_lineRenderer.gameObject.SetActive(true);
    }

    public void HideLine()
    {
        m_lineRenderer.gameObject.SetActive(false);
    }

    public void SetColourGradient(Gradient colourGradient)
    {
        m_lineRenderer.colorGradient = colourGradient;
    }
}
