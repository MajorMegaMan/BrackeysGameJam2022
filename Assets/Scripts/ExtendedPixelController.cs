using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSX;

public class ExtendedPixelController : PixelationController
{
    [SerializeField] Camera m_camera;
    [SerializeField] float m_targetWidth = 1920;
    [SerializeField] float m_pixelRatio = 2.0f;

    int lastWidth;
    int lastHeight;

    public float pixelRatio { get { return m_pixelRatio; } 
        set 
        { 
            m_pixelRatio = value;
            UpdateSizeVar();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateLastCamSize();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(UpdateLastCamSize())
        {
            UpdateSizeVar();
        }
    }

    void UpdateSizeVar()
    {
        float widthResolutionScale = m_targetWidth / lastWidth;
        widthPixelation = (lastWidth / m_pixelRatio) * widthResolutionScale;
        heightPixelation = (lastHeight / m_pixelRatio) * widthResolutionScale;
    }

    public bool UpdateLastCamSize()
    {
        return UpdateLastCamSize(m_camera);
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
            UpdateLastCamSize();

            UpdateSizeVar();
        }
    }
}
