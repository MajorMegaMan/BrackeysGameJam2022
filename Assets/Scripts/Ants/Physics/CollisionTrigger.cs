using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    [SerializeField] UnityEvent<Collider> m_collisionEvent;

    [SerializeField] Collider m_triggerCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        m_collisionEvent.Invoke(other);
    }

    public Vector3 CalculateVelocity(Rigidbody rigidbody)
    {
        return rigidbody.GetPointVelocity(m_triggerCollider.ClosestPoint(rigidbody.position));
    }
}
