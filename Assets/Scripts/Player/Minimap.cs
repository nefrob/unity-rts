using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Mirror;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private Transform mapPlane = null;

    private const int PLANE_SCALE = 10;

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect,
            mousePos,
            null,
            out Vector2 localPoint
        )) { return; }

        Vector2 lerp = new Vector2(
            (localPoint.x - minimapRect.rect.x) / minimapRect.rect.width,
            (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);

        float planeOffsetX = mapPlane.localScale.x * PLANE_SCALE / 2;
        float planeOffsetZ = mapPlane.localScale.z * PLANE_SCALE / 2;
        Vector3 newCameraPos = new Vector3(
            Mathf.Lerp(-planeOffsetX, planeOffsetX, lerp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-planeOffsetZ, planeOffsetZ, lerp.y));

        playerCameraTransform.position = newCameraPos;
    }
}
