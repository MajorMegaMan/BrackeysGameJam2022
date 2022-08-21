using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public InputReceiver inputReceiver = null;

    [SerializeField] CharacterController m_characterControl = null;

    [SerializeField] float m_speed = 5.0f;

    Vector3 m_moveInput = Vector3.zero;

    Vector3 m_velocity = Vector3.zero;

    public float speed { get { return m_speed; } }
    public Vector3 velocity { get { return m_velocity; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();

        m_velocity = m_moveInput * m_speed;

        m_characterControl.Move(m_velocity * Time.deltaTime);
    }

    void GetInputs()
    {
        m_moveInput = inputReceiver.GetMovement();
    }
}
