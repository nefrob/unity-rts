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
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float moveBoarder = 50f;

    [SerializeField] private Transform camTransform = null;

    [SerializeField] private Transform mapPlane = null;
    [SerializeField] private float mapCameraEdgeBorder = 20.0f;

    // Target to follow
    private Transform followTransform;

    private Rect leftRect;
    private Rect rightRect;
    private Rect upRect;
    private Rect downRect;

    private Controls controls;
    private Vector2 previousMoveInput;
    private float previousScrollInput;
    private float previousRotateInput;

    private Vector3 mapMinBounds;
    private Vector3 mapMaxBounds;

    private const int PLANE_SCALE = 10;

    void Start()
    {
        followTransform = null;
        leftRect = new Rect(0, 0, moveBoarder, Screen.height);
        rightRect = new Rect(Screen.width - moveBoarder, 0, moveBoarder, Screen.height);
        upRect = new Rect(0, Screen.height - moveBoarder, Screen.width, moveBoarder);
        downRect = new Rect(0, 0, Screen.width, moveBoarder);

        float planeOffsetX = mapPlane.localScale.x * PLANE_SCALE / 2;
        float planeOffsetZ = mapPlane.localScale.z * PLANE_SCALE / 2;
        mapMinBounds = new Vector3(mapPlane.position.x - planeOffsetX, 0, mapPlane.position.z - planeOffsetZ);
        mapMaxBounds = new Vector3(mapPlane.position.x + planeOffsetX, 0, mapPlane.position.z + planeOffsetZ);
        mapMinBounds.x += mapCameraEdgeBorder;
        mapMinBounds.z += mapCameraEdgeBorder;
        mapMaxBounds.x -= mapCameraEdgeBorder;
        mapMaxBounds.z -= mapCameraEdgeBorder;

        controls = new Controls();
        controls.Player.MoveCamera.performed += SetMoveInput;
        controls.Player.MoveCamera.canceled += SetMoveInput;
        controls.Player.ZoomCamera.performed += SetScrollInput;
        controls.Player.ZoomCamera.canceled += SetScrollInput;
        controls.Player.RotateCamera.performed += SetRotateInput;
        controls.Player.RotateCamera.canceled += SetRotateInput;
        controls.Enable();
    }

    void Update()
    {
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        } else 
        {
            if (!Application.isFocused) return;

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
        Vector3 movePos = transform.position;

        int vert = 0;
        int hor = 0;
        if (useKeyBoardMove)
        {
            vert = (int) previousMoveInput.y;
            hor = (int) previousMoveInput.x;
        } else
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            vert = upRect.Contains(mousePos) ? 1 : downRect.Contains(mousePos) ? -1 : 0;
            hor = leftRect.Contains(mousePos) ? -1 : rightRect.Contains(mousePos) ? 1 : 0;
        }

        if (vert == 0 && hor == 0) return;

        movePos += transform.forward * vert * moveSpeed * Time.deltaTime;
        movePos += transform.right * hor * moveSpeed * Time.deltaTime;

        // Clamp camera movement at screen edges
        movePos.x = Mathf.Clamp(movePos.x, mapMinBounds.x, mapMaxBounds.x);
        movePos.z = Mathf.Clamp(movePos.z, mapMinBounds.z, mapMaxBounds.z);

        transform.position = movePos;
    }

    private void Zoom()
    {
        float scrollInput = previousScrollInput;
        if (scrollInput == 0) return;

        float dist = Vector3.Distance(transform.position, 
            camTransform.position);

        if ((dist <= minZoom && scrollInput > 0.0f)
            || (dist >= maxZoom && scrollInput < 0.0f))
        {
            return;
        }

        Vector3 zoomPos = camTransform.position;
        zoomPos += scrollInput * zoomSpeed * camTransform.forward;

        // Clamp check
        dist = Vector3.Distance(transform.position, 
            camTransform.position);

        if (dist <= minZoom && scrollInput > 0.0f)
        {
            zoomPos = transform.position - minZoom * camTransform.forward.normalized;
        } else if (dist >= maxZoom && scrollInput < 0.0f)
        {
            zoomPos = transform.position - maxZoom * camTransform.forward.normalized;
        }

        camTransform.position = Vector3.Lerp(
            camTransform.position, zoomPos, Time.deltaTime);
    }

    private void Rotate()
    {
        float rotateInput = previousRotateInput;
        if (rotateInput == 0) return;

        Quaternion newRot = transform.rotation;
        newRot *= Quaternion.Euler(Vector3.up * rotateInput * rotateSpeed);

        transform.rotation = Quaternion.Lerp(transform.rotation,
            newRot, Time.deltaTime);
    }

    public void FocusTarget(Transform target)
    {
        followTransform = target;
        transform.position = followTransform.position;
    }

    private void SetMoveInput(InputAction.CallbackContext ctx)
    {
        previousMoveInput = ctx.ReadValue<Vector2>();
    }

    private void SetScrollInput(InputAction.CallbackContext ctx)
    {
        previousScrollInput = ctx.ReadValue<float>();
    }

    private void SetRotateInput(InputAction.CallbackContext ctx)
    {
        previousRotateInput = ctx.ReadValue<float>();
    }
}
