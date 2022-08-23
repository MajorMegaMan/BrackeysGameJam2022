using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnimator : MonoBehaviour
{
    [SerializeField] Animator m_anim;
    [SerializeField] Rigidbody[] m_rigidBodies;

    [SerializeField] bool m_isActive = false;

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
