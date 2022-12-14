using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    public Transform lookTarget = null;
    public Transform targetFollow = null;
    public float distance = 5.0f;

    Vector3 m_smoothPosVelocity = Vector3.zero;
    [SerializeField] float m_smoothPosTime = 0.1f;

    Vector3 m_smoothLookPosition = Vector3.zero;
    Vector3 m_smoothLookVelocity = Vector3.zero;
    [SerializeField] float m_smoothLookTime = 0.1f;

    [SerializeField] float m_maxHeight = 3.0f;
    [SerializeField] float m_minHeight = 2.0f;

    [SerializeField] LayerMask m_collisionLayers = ~0;

    public float smoothPosTime { get { return m_smoothPosTime; } set { m_smoothPosTime = value; } }
    public float smoothLookTime { get { return m_smoothLookTime; } set { m_smoothLookTime = value; } }

    // Update is called once per frame
    void Update()
    {
        if(targetFollow != null)
        {
            Vector3 toTarget = targetFollow.position - transform.position;

            Vector3 targetPos;
            if(Physics.Raycast(targetFollow.position, -toTarget, out RaycastHit hitInfo, distance, m_collisionLayers, QueryTriggerInteraction.Ignore))
            {
                targetPos = hitInfo.point;
            }
            else
            {
                targetPos = targetFollow.position - toTarget.normalized * distance;
            }

            Vector3 clampedPos = targetPos;
            clampedPos.y = Mathf.Clamp(clampedPos.y, m_minHeight + targetFollow.position.y, m_maxHeight + targetFollow.position.y);
            transform.position = Vector3.SmoothDamp(transform.position, clampedPos, ref m_smoothPosVelocity, m_smoothPosTime);
        }

        if(lookTarget != null)
        {
            m_smoothLookPosition = Vector3.SmoothDamp(m_smoothLookPosition, lookTarget.position, ref m_smoothLookVelocity, m_smoothLookTime);
            transform.LookAt(m_smoothLookPosition);
        }
    }

    public void SetTargetFollow(Transform followTarget)
    {
        targetFollow = followTarget;
    }

    public void SetLookTarget(Transform lookTarget)
    {
        this.lookTarget = lookTarget;
    }
}
