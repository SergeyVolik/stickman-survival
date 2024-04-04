using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform CameraTrans { get; private set; }

    private void Awake()
    {
        CameraTrans = Camera.main.transform;
    }
    
    void LateUpdate()
    {
        transform.forward = CameraTrans.forward;
    }
}
