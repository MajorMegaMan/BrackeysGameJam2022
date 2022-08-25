using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseIndicator : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager = null;

    [SerializeField] RaySelector m_raySelector = null;
    [SerializeField] BuildLineTool m_lineTool = null;

    [SerializeField] float m_selectorDistance = 5.0f;

    [SerializeField] LayerMask m_carryObjectLayerMask = 0;

    [SerializeField] MeshRenderer m_meshRenderer;
    [SerializeField] Color m_validColour = Color.green;
    [SerializeField] Color m_invalidColour = Color.red;
    [SerializeField] float m_coverSkinWidth = 0.1f;

    Material m_selectorMaterial = null;

    Collider m_lastHitCollider = null;
    Vector3 m_lastHitpoint = Vector3.zero;
    GridManager.GridCellKey m_lastCellKey;
    Vector3 m_lastCellPosition = Vector3.zero;

    public Collider lastHitCollider { get { return m_lastHitCollider; } }
    public Vector3 lastHitpoint { get { return m_lastHitpoint; } }
    public GridManager.GridCellKey lastCellKey { get { return m_lastCellKey; } }
    public Vector3 lastCellPosition { get { return m_lastCellPosition; } }

    private void Awake()
    {
        m_selectorMaterial = m_meshRenderer.material;
    }

    private void Start()
    {
        SetSelectionColour(true);
    }

    public bool CamRayCast()
    {
        bool hit = m_raySelector.CamRayCast(out Ray camRay, out RaycastHit hitInfo, m_selectorDistance);

        if (hit)
        {
            ValidateLastHitInfo(hitInfo.collider, hitInfo.point, camRay.direction);
        }
        else
        {
            //ValidateLastHitInfo(null, camRay.origin + camRay.direction * m_selectorDistance, camRay.direction);
        }

        return hit;
    }

    void ValidateLastHitInfo(Collider hitcollider, Vector3 hitPoint, Vector3 rayDir)
    {
        m_lastHitCollider = hitcollider;
        m_lastHitpoint = hitPoint - rayDir * m_raySelector.raySkin;

        m_lastCellKey = m_gridManager.GetCellKey(m_lastHitpoint);
        m_lastCellPosition = m_gridManager.GetCellPosition(m_lastCellKey);

        Vector3 mouseHighlightPosition = m_lastCellPosition;
        Vector3 scale = transform.localScale;
        Vector3 halfOffset = Vector3.one * m_gridManager.cellSize * 0.5f;

        mouseHighlightPosition.y -= scale.y * 0.5f;
        mouseHighlightPosition.y += scale.y - halfOffset.y;

        if (!IsOdd((int)scale.x))
        {
            mouseHighlightPosition.x += -Mathf.Sign(Vector3.Dot(Camera.main.transform.forward, Vector3.forward)) * halfOffset.x;
        }

        if (!IsOdd((int)scale.z))
        {
            mouseHighlightPosition.z += -Mathf.Sign(Vector3.Dot(Camera.main.transform.right, Vector3.right)) * halfOffset.z;
        }

        transform.position = mouseHighlightPosition;
    }

    bool IsOdd(int number)
    {
        return number % 2 != 0;
    }

    void SetPositionToLastHitCollider()
    {
        if(m_lastHitCollider != null)
        {
            transform.position = m_lastHitCollider.transform.position;
        }
    }

    public void CoverLastHitCollider()
    {
        SetPositionToLastHitCollider();
        SetSelectorScale(m_lastHitCollider.bounds.size + Vector3.one * m_coverSkinWidth);
    }

    public bool IsLastHitColliderCarriable()
    {
        return m_lastHitCollider != null && Utility.ContainsLayer(m_lastHitCollider.gameObject.layer, m_carryObjectLayerMask);
    }

    public void SetSelectorScale(Vector3 scale)
    {
        transform.localScale = scale;
    }

    public void SetSelectorScaleToGridCellSize()
    {
        transform.localScale = Vector3.one * m_gridManager.cellSize;
    }

    public void StartDrawLine()
    {
        m_lineTool.SetStart(m_lastCellPosition);
        m_lineTool.SetEnd(m_lastCellPosition + Vector3.up * 0.001f);
        m_lineTool.ShowLine();
    }

    public void SetEndDrawLine()
    {
        m_lineTool.SetEnd(m_lastCellPosition);
    }

    public void FinishDrawLine()
    {
        m_lineTool.SetEnd(m_lastCellPosition);
        m_lineTool.HideLine();
    }

    public Vector3 GetLineStart()
    {
        return m_lineTool.GetStart();
    }

    public Vector3 GetLineEnd()
    {
        return m_lineTool.GetEnd();
    }

    Color CopyRGB(Color target, float a)
    {
        Color result = target;
        result.a = a;
        return result;
    }

    public void SetSelectionColour(bool isValid)
    {
        Color baseColour = m_selectorMaterial.GetColor("_Colour");
        Color lineColour = m_selectorMaterial.GetColor("_LineColour");
        if (isValid)
        {
            baseColour = CopyRGB(m_validColour, baseColour.a);
            lineColour = CopyRGB(m_validColour, lineColour.a);
        }
        else
        {
            baseColour = CopyRGB(m_invalidColour, baseColour.a);
            lineColour = CopyRGB(m_invalidColour, lineColour.a);
        }

        m_selectorMaterial.SetColor("_Colour", baseColour);
        m_selectorMaterial.SetColor("_LineColour", lineColour);
    }

    private void OnValidate()
    {
        if(Application.isPlaying && m_selectorMaterial != null)
        {
            SetSelectionColour(true);
        }
    }
}
