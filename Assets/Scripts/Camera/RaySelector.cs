using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaySelector : MonoBehaviour
{
    [SerializeField] Camera m_camera = null;
    [SerializeField] LayerMask m_camRayLayer = 0;

    [SerializeField] float m_raySkin = 0.002f;

    public float raySkin { get { return m_raySkin; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool CamRayCast(out Ray camRay, out RaycastHit hitInfo)
    {
        camRay = m_camera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(camRay, out hitInfo, float.PositiveInfinity, m_camRayLayer, QueryTriggerInteraction.Ignore);
    }

    public bool CamRayCast(out RaycastHit hitInfo)
    {
        return CamRayCast(out Ray ray, out hitInfo);
    }

    public bool CamRayCastPoint(out Vector3 point)
    {
        bool hit = CamRayCast(out Ray ray, out RaycastHit hitInfo);
        point = hitInfo.point - ray.direction * m_raySkin;
        return hit;
    }
}
