using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugposeReset : MonoBehaviour
{
    [SerializeField] RagdollAnimator m_ragdoll = null;

    Vector3[] m_positions = null;
    Quaternion[] m_rotations = null;

    private void Awake()
    {
        InitPose();
        m_ragdoll.onRagdollChange.AddListener(TogglePose);
    }

    private void OnDestroy()
    {
        m_ragdoll.onRagdollChange.RemoveListener(TogglePose);
    }

    void InitPose()
    {
        var rigidbodys = m_ragdoll.rigidBodies;
        m_positions = new Vector3[rigidbodys.Length];
        m_rotations = new Quaternion[rigidbodys.Length];

        for (int i = 0; i < rigidbodys.Length; i++)
        {
            m_positions[i] = rigidbodys[i].transform.localPosition;
            m_rotations[i] = rigidbodys[i].transform.localRotation;
        }
    }

    public void ResetToPose()
    {
        var rigidbodys = m_ragdoll.rigidBodies;

        for (int i = 0; i < rigidbodys.Length; i++)
        {
            rigidbodys[i].transform.localPosition = m_positions[i];
            rigidbodys[i].transform.localRotation = m_rotations[i];
        }
    }

    public void TogglePose(bool ragdollActive)
    {
        if(!ragdollActive)
        {
            ResetToPose();
        }
    }
}
