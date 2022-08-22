using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpin : MonoBehaviour
{
    [SerializeField] float m_camSpeed = 1.0f;

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

            Vector3 moveDir = transform.right * mouseInput.x * m_camSpeed;
            moveDir += transform.up * mouseInput.y * m_camSpeed;
            transform.position += moveDir;
        }
    }
}
