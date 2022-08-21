using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMouseHighlight : MonoBehaviour
{
    [SerializeField] RaySelector m_raySelector;
    [SerializeField] GridManager m_gridManager;

    private void Update()
    {
        if(m_raySelector.CamRayCastPoint(out Vector3 hitPoint))
        {
            Vector3 cellPos = m_gridManager.GetCellPosition(m_gridManager.GetCellKey(hitPoint));
            SetPosition(cellPos);
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
