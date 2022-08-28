using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent m_winEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toPlayer = GameManager.instance.player.transform.position - transform.position;
        if(toPlayer.magnitude < 3.0f)
        {
            m_winEvent.Invoke();
            Destroy(this);
        }
    }
}
