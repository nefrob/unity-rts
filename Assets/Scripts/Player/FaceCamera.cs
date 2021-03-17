using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] private Transform camTransform = null;

    private void Start()
    {
        if (camTransform == null)
        {
            camTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        // FIXME: lookat gives weird behavior, using euler angles instead
        // transform.LookAt(
        //     transform.position + camTransform.rotation * Vector3.forward,
        //     camTransform.rotation * Vector3.up);
        transform.eulerAngles  = camTransform.eulerAngles; 
    }
}
