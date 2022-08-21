using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    [SerializeField] Transform m_viewInputTransform = null;

    public Vector3 GetMovement()
    {
        Vector3 movement = Vector3.zero;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        return ConvertInput(movement);
    }

    Vector3 ConvertInput(Vector3 input)
    {
        Vector3 result = m_viewInputTransform.TransformVector(input);
        result.y = 0.0f;
        result = result.normalized * input.magnitude;
        return result;
    }
}
