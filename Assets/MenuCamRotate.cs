using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCamRotate : MonoBehaviour
{
    [SerializeField] float m_rotateSpeed = 20.0f;

    [SerializeField] bool m_isRotating = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isRotating)
        {
            Vector3 euler = transform.eulerAngles;
            euler.y += m_rotateSpeed * Time.deltaTime;
            transform.eulerAngles = euler;
        }
    }
}
