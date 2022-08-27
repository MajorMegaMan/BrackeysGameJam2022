using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSX;

public class ExtendedPixelController : PixelationController
{
    [SerializeField] Camera m_camera;
    [SerializeField] float m_pixelRatio = 2.0f;

    int lastWidth;
    int lastHeight;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLastCamSize(m_camera);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(UpdateLastCamSize(m_camera))
        {
            widthPixelation = lastWidth / m_pixelRatio;
            heightPixelation = lastHeight / m_pixelRatio;
        }
    }

    bool UpdateLastCamSize(Camera cam)
    {
        bool hasChanged = lastWidth != cam.pixelWidth || lastHeight != cam.pixelHeight;

        lastWidth = cam.pixelWidth;
        lastHeight = cam.pixelHeight;
        return hasChanged;
    }

    private void OnValidate()
    {
        if (m_camera != null)
        {
            UpdateLastCamSize(m_camera);

            widthPixelation = lastWidth / m_pixelRatio;
            heightPixelation = lastHeight / m_pixelRatio;
        }
    }
}
