using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //
    [SerializeField] bool useKeyBoardMove = true;

    //
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float edgeMoveSpeed = 50f;
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float maxZoom = 20f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float moveBoarder = 50f;

    //
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    private void Move()
    {
        if (useKeyBoardMove)
        {
            Vector3 movePos = transform.position;
            movePos += transform.forward * Input.GetAxis("Vertical");
            movePos += transform.right * Input.GetAxis("Horizontal");

            transform.position = Vector3.Lerp(transform.position,
                movePos, moveSpeed * Time.deltaTime);
        } else
        {
            Vector3 movePos = transform.position;
            Vector2 mousePos = Input.mousePosition;

            Rect leftRect = new Rect(0, 0, moveBoarder, Screen.height);
            Rect rightRect = new Rect(Screen.width - moveBoarder, 0, moveBoarder, Screen.height);
            Rect upRect = new Rect(0, Screen.height - moveBoarder, Screen.width, moveBoarder);
            Rect downRect = new Rect(0, 0, Screen.width, moveBoarder);

            movePos.x += leftRect.Contains(mousePos) ? -1 : rightRect.Contains(mousePos) ? 1 : 0;
            movePos.z += upRect.Contains(mousePos) ? 1 : downRect.Contains(mousePos) ? -1 : 0;

            transform.position = Vector3.Lerp(transform.position,
                movePos, edgeMoveSpeed * Time.deltaTime);
        }
    }

    private void Zoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        float dist = Vector3.Distance(transform.position, 
            Camera.main.transform.position);

        if ((dist <= minZoom && scrollInput > 0.0f) 
            || (dist >= maxZoom && scrollInput < 0.0f))
        {
            return;
        }

        Vector3 zoomPos = Camera.main.transform.position;
        zoomPos += scrollInput * zoomSpeed * Camera.main.transform.forward; 
        Camera.main.transform.position = Vector3.Lerp(
            Camera.main.transform.position, zoomPos, Time.deltaTime);
    }

    private void Rotate()
    {
        Quaternion newRot = transform.rotation;
        if (Input.GetKey(KeyCode.Q))
        {
            newRot *= Quaternion.Euler(Vector3.up * rotateSpeed);
        }
        if (Input.GetKey(KeyCode.E))
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
