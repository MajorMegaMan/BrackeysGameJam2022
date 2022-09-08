using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MouseIndicator : MonoBehaviour
{
    [SerializeField] GridManager m_gridManager = null;

    [SerializeField] RaySelector m_raySelector = null;
    [SerializeField] BuildLineTool m_lineTool = null;

    [SerializeField] float m_selectorDistance = 5.0f;

    [SerializeField] LayerMask m_carryObjectLayerMask = 0;
    [SerializeField] LayerMask m_bridgeLayerMask = 0;

    [SerializeField] MeshRenderer m_meshRenderer;
    [SerializeField] Color m_validColour = Color.green;
    [SerializeField] Color m_invalidColour = Color.red;
    [SerializeField] Color m_bridgeColour = Color.yellow;
    [SerializeField] float m_coverSkinWidth = 0.1f;

    [Header("UI")]
    [SerializeField] GameObject m_selectedAntCountCanvas = null;
    [SerializeField] TMP_Text m_selectedAntCountText = null;

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
        SetSelectionColour(SelectionColour.valid);
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

    public void CoverLastHitCarriableCollider()
    {
        SetPositionToLastHitCollider();
        SetSelectorScale(m_lastHitCollider.bounds.size + Vector3.one * m_coverSkinWidth);
    }

    public void CoverLastHitBridgeCollider()
    {
        SetPositionToLastHitCollider();
        var capsuleCollider = ((CapsuleCollider)m_lastHitCollider);
        Vector3 centerOffset = m_lastHitCollider.transform.localToWorldMatrix * capsuleCollider.center;
        transform.position += centerOffset;
        transform.up = m_lastHitCollider.transform.up;
        Vector3 localScale = transform.localScale;
        localScale.y = capsuleCollider.height;
        transform.localScale = localScale;
    }

    public bool IsLastHitColliderCarriable()
    {
        return m_lastHitCollider != null && Utility.ContainsLayer(m_lastHitCollider.gameObject.layer, m_carryObjectLayerMask);
    }

    public bool IsLastHitColliderBridge()
    {
        return m_lastHitCollider != null && Utility.ContainsLayer(m_lastHitCollider.gameObject.layer, m_bridgeLayerMask);
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

    public Vector3 GetLineDir()
    {
        return m_lineTool.GetEnd() - m_lineTool.GetStart();
    }

    Color CopyRGB(Color target, float a)
    {
        Color result = target;
        result.a = a;
        return result;
    }

    public enum SelectionColour
    {
        valid,
        inValid,
        bridge
    }

    public void SetSelectionColour(SelectionColour selectionColour)
    {
        Color baseColour = m_selectorMaterial.GetColor("_Colour");
        Color lineColour = m_selectorMaterial.GetColor("_LineColour");

        switch(selectionColour)
        {
            case SelectionColour.valid:
                {
                    baseColour = CopyRGB(m_validColour, baseColour.a);
                    lineColour = CopyRGB(m_validColour, lineColour.a);
                    break;
                }
            case SelectionColour.inValid:
                {
                    baseColour = CopyRGB(m_invalidColour, baseColour.a);
                    lineColour = CopyRGB(m_invalidColour, lineColour.a);
                    break;
                }
            case SelectionColour.bridge:
                {
                    baseColour = CopyRGB(m_bridgeColour, baseColour.a);
                    lineColour = CopyRGB(m_bridgeColour, lineColour.a);
                    break;
                }
        }

        m_selectorMaterial.SetColor("_Colour", baseColour);
        m_selectorMaterial.SetColor("_LineColour", lineColour);
    }

    public void SetSelectionColour(bool isValid)
    {
        if(isValid)
        {
            SetSelectionColour(SelectionColour.valid);
        }
        else
        {
            SetSelectionColour(SelectionColour.inValid);
        }
    }

    public void EnableText(bool enabled)
    {
        m_selectedAntCountCanvas.SetActive(enabled);
    }

    public void SetTextPosition(Vector3 position)
    {
        m_selectedAntCountCanvas.transform.position = position;
    }

    public void TextLookAt(Transform target)
    {
        m_selectedAntCountCanvas.transform.LookAt(target);
    }

    public void SetAntCountText(int antCount)
    {
        m_selectedAntCountText.text = antCount.ToString();
    }

    public void UpdateTextForLine(Transform lookAt, int lineAntCount)
    {
        Vector3 lineDir = GetLineDir();
        SetTextPosition(GetLineStart() + lineDir * 0.5f + Vector3.up * 1.0f);
        SetAntCountText(lineAntCount);
        TextLookAt(lookAt);
    }

    public void UpdateTextForLine(Transform lookAt)
    {
        Vector3 lineDir = GetLineDir();
        float lineMagnitude = lineDir.magnitude;
        UpdateTextForLine(lookAt, AntManager.instance.CalculateLineAntCount(lineMagnitude));
    }

    public void UpdateTextForCarryObject(Transform lookAt, CarryableObject carryableObject)
    {
        TextLookAt(lookAt);
        SetAntCountText(carryableObject.antStrengthCount);
        SetTextPosition(carryableObject.GetTextPosition());
    }

    private void OnValidate()
    {
        if(Application.isPlaying && m_selectorMaterial != null)
        {
            SetSelectionColour(SelectionColour.valid);
        }
    }
}
