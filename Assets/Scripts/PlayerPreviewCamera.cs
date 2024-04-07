using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPreviewCamera : MonoBehaviour
{
    [SerializeField]
    private Camera m_PreviewCamera;
    public Transform cameraTarget;
    private void Awake()
    {
        Dectivate();
        
    }

    public void Activate()
    {
        m_PreviewCamera.gameObject.SetActive(true);
    }

    public void Dectivate()
    {
        m_PreviewCamera.gameObject.SetActive(false);
    }

    public void RotateCamera(float rotateAngle)
    {
        m_PreviewCamera.transform.RotateAround(cameraTarget.position, Vector3.up, rotateAngle);
        //m_PreviewCamera.transform.LookAt(cameraTarget.position);

    }
}
