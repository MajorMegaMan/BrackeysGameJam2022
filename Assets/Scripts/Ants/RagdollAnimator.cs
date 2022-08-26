using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RagdollAnimator : MonoBehaviour
{
    [SerializeField] Animator m_anim;
    [SerializeField] Rigidbody[] m_rigidBodies;

    [SerializeField] bool m_isActive = false;

    [SerializeField] UnityEvent<bool> m_onRagdollChange = null;

    public Rigidbody[] rigidBodies { get { return m_rigidBodies; } }
    public UnityEvent<bool> onRagdollChange { get { return m_onRagdollChange; } }

    private void Awake()
    {
        Activate(m_isActive);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate(bool isActive)
    {
        m_isActive = isActive;
        m_anim.enabled = !isActive;
        for(int i = 0; i < m_rigidBodies.Length; i++)
        {
            m_rigidBodies[i].gameObject.SetActive(isActive);
        }

        m_onRagdollChange.Invoke(isActive);
    }

    public Rigidbody GetRigidbody(int index)
    {
        return m_rigidBodies[index];
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            Activate(m_isActive);
        }
    }
}
