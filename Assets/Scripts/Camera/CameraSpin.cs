using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    [SerializeField] float m_camSpeed = 1.0f;

    Vector3 m_moveSmooth = Vector3.zero;
    Vector3 m_moveSmoothVel = Vector3.zero;
    [SerializeField] float m_moveSmoothTime = 0.1f;

    public bool invertX = false;
    public bool invertY = false;

    public float camSpeed { get { return m_camSpeed; } set { m_camSpeed = value; } }

    public Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(1))
        {
            Vector2 mouseInput = Vector2.zero;
            mouseInput.x = Input.GetAxisRaw("Mouse X");
            mouseInput.y = Input.GetAxisRaw("Mouse Y");

            if (invertX)
                mouseInput.x *= -1;
            if (invertY)
                mouseInput.y *= -1;

            Vector3 moveDir = transform.right * mouseInput.x * m_camSpeed;
            moveDir += transform.up * mouseInput.y * m_camSpeed;

            m_moveSmooth = Vector3.SmoothDamp(m_moveSmooth, moveDir, ref m_moveSmoothVel, m_moveSmoothTime);

            transform.position += m_moveSmooth;
        }

        Vector3 dir = transform.localToWorldMatrix * velocity;
        transform.position += dir * Time.deltaTime;
    }

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }
}
