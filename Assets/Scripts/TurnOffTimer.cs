using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffTimer : MonoBehaviour
{
    [SerializeField] float m_time = 5.0f;
    float m_timer = 0.0f;

    public bool isEnabled = false;

    [SerializeField] Rigidbody targetDestroy;
 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled)
        {
            m_timer += Time.deltaTime;
            if(m_timer > m_time)
            {
                targetDestroy.gameObject.layer = 0;
                Destroy(targetDestroy);
                Destroy(this);
            }
        }
    }

    public void EnableTimer()
    {
        isEnabled = true;
    }
}
