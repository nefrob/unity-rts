using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    // Whether to use keyboard or mouse to move Camera
    [SerializeField] bool useKeyBoardMove = true;

    // Movement speeds
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float edgeMoveSpeed = 50f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float moveBoarder = 50f;

    [SerializeField] private Transform camTransform = null;

    // Target to follow
    private Transform followTransform; 

    void Start()
    {
        followTransform = null;
    }

    void Update()
    {
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        } else 
        {
            Move();
            Zoom();
            Rotate();
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            followTransform = null;
        }
    }

    private void Move()
    {
        if (useKeyBoardMove)
        {
            Vector3 movePos = transform.position;
            int vert = Keyboard.current.wKey.isPressed ? 1 : (Keyboard.current.sKey.isPressed ? -1 : 0);
            int hor = Keyboard.current.dKey.isPressed ? 1 : (Keyboard.current.aKey.isPressed ? -1 : 0);
            movePos += transform.forward * vert;
            movePos += transform.right * hor;

            transform.position = Vector3.Lerp(transform.position, // FIXME: clamp
                movePos, moveSpeed * Time.deltaTime);
        } else
        {
            Vector3 movePos = transform.position;
            Vector2 mousePos = Mouse.current.position.ReadValue();

            Rect leftRect = new Rect(0, 0, moveBoarder, Screen.height);
            Rect rightRect = new Rect(Screen.width - moveBoarder, 0, moveBoarder, Screen.height);
            Rect upRect = new Rect(0, Screen.height - moveBoarder, Screen.width, moveBoarder);
            Rect downRect = new Rect(0, 0, Screen.width, moveBoarder);

            movePos.x += leftRect.Contains(mousePos) ? -1 : rightRect.Contains(mousePos) ? 1 : 0;
            movePos.z += upRect.Contains(mousePos) ? 1 : downRect.Contains(mousePos) ? -1 : 0;

            transform.position = Vector3.Lerp(transform.position, // FIXME: clamp
                movePos, edgeMoveSpeed * Time.deltaTime);
        }
    }

    private void Zoom()
    {
        float scrollInput = Mouse.current.scroll.y.ReadValue();
        if (scrollInput == 0) return;

        float dist = Vector3.Distance(transform.position, 
            camTransform.position);

        if ((dist <= minZoom && scrollInput > 0.0f) 
            || (dist >= maxZoom && scrollInput < 0.0f))
        {
            return;
        }

        Vector3 zoomPos = camTransform.position;
        zoomPos += scrollInput * zoomSpeed * camTransform.forward;  // FIXME: clamp

        camTransform.position = Vector3.Lerp(
            camTransform.position, zoomPos, Time.deltaTime);
    }

    private void Rotate()
    {
        Quaternion newRot = transform.rotation;
        if (Keyboard.current.qKey.isPressed)
        {
            newRot *= Quaternion.Euler(Vector3.up * rotateSpeed);
        }
        if (Keyboard.current.eKey.isPressed)
        {
            newRot *= Quaternion.Euler(Vector3.up * -rotateSpeed);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation,
            newRot, Time.deltaTime);
    }

    public void FocusTarget(Transform target)
    {
        followTransform = target;
        transform.position = followTransform.position;
    }
}
