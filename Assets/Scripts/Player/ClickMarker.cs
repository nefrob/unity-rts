using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClickMarker : MonoBehaviour
{
    // Marker display
    // FIXME: loop in with selection/action managers for different sprites
    [SerializeField] private Image arrow = null;
    [SerializeField] private float displayTime = 1.0f;
    private float remainingDiplayTime;

    // Ground positioning
    [SerializeField] private LayerMask groundMask = 0;
    [SerializeField] private Camera overlayCam = null;

    void Awake()
    {
        remainingDiplayTime = 0.0f;
    }

    void Update()
    {
        // Countdown visibility time
        remainingDiplayTime -= Time.deltaTime;
        if (remainingDiplayTime <= 0)
        {
            arrow.enabled = false;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = overlayCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,  Mathf.Infinity, groundMask))
            {
                Vector3 pos = hit.point;
                pos.y = transform.position.y;
                transform.position = pos;

                remainingDiplayTime = displayTime;
                arrow.enabled = true;

                // FIXME: tween sprite?
            }
        }
    }
}
