using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SphereCollider))]
public class PickUpTrigger : MonoBehaviour
{
    [SerializeField] LayerMask m_targetTriggerLayers = ~0;

    [SerializeField] UnityEvent m_pickUpEvent;

    private void Start()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = AntManager.instance.settings.pickUpRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Utility.ContainsLayer(other.gameObject.layer, m_targetTriggerLayers))
        {
            m_pickUpEvent.Invoke();
        }
    }
}
