using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamLookTarget : MonoBehaviour
{
    [SerializeField] Transform m_followTarget;

    [SerializeField] Transform[] m_splitFollow;

    delegate void FollowDelegate();
    FollowDelegate m_followDelegate;

    [SerializeField] bool m_modeIsSingle = true;

    private void Awake()
    {
        m_followDelegate = FollowSingle;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_followDelegate.Invoke();
    }

    void FollowSingle()
    {
        transform.position = m_followTarget.position;
    }

    void FollowSplit()
    {
        Vector3 averagePos = m_followTarget.position;
        for (int i = 0; i < m_splitFollow.Length; i++)
        {
            averagePos += m_splitFollow[i].position;
        }

        averagePos /= m_splitFollow.Length + 1;

        transform.position = averagePos;
    }

    public void SetMode(bool isSingle)
    {
        if(isSingle)
        {
            m_followDelegate = FollowSingle;
        }
        else
        {
            m_followDelegate = FollowSplit;
        }
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            SetMode(m_modeIsSingle);
        }
    }
}
