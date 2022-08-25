using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCollider : MonoBehaviour
{
    [SerializeField] CapsuleCollider m_capsuleCollider;

    public void SetRotation(Vector3 lineDir)
    {
        transform.up = lineDir;
    }

    public void SetLength(float length)
    {
        m_capsuleCollider.height = length + m_capsuleCollider.radius * 2;
        Vector3 center = m_capsuleCollider.center;
        center.y = m_capsuleCollider.height * 0.5f - m_capsuleCollider.radius;
        m_capsuleCollider.center = center;
    }

    public void Disassemble()
    {
        // For now just destroy the object
        Destroy(gameObject);
    }
}
