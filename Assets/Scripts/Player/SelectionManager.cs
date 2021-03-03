﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    // UI selection box
    [SerializeField] private RectTransform selectionBox = null;
    private Vector2 boxStartPos;

    // Selection layer mask
    [SerializeField] private LayerMask selectableMask = 0;

    // Tracking selected objects
    private HashSet<Selectable> selectedObjects;
    private UnitManager unitManager;

    void Awake()
    {
        selectedObjects = new HashSet<Selectable>();
        unitManager = FindObjectOfType<UnitManager>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            boxStartPos = Input.mousePosition;
            TrySelect(Input.mousePosition, Input.GetKey(KeyCode.LeftShift));
        }
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    private void TrySelect(Vector2 position, bool shiftHeld)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableMask))
        {
            var s = hit.transform.GetComponent<Selectable>();

            // FIXME: check unit ownership
            // FIXME: check selectables are same type (ex. not resource and unit)
            if (shiftHeld && selectedObjects.Contains(s))
            {
                selectedObjects.Remove(s);
                s.DeselectObject();
                return;
            }
            
            if (!shiftHeld)
                DeselectAll();

            selectedObjects.Add(s);
            s.SelectObject();   
        } else
        {
            DeselectAll();
        }
    }

    private void DeselectAll()
    {
        foreach (Selectable s in selectedObjects)
        {
            s.DeselectObject();
        }
        selectedObjects.Clear();
    }

    private void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Selectable s in unitManager.allObjects)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(s.gameObject.transform.position);
            if (screenPos.x > min.x && screenPos.x < max.x 
                && screenPos.y > min.y && screenPos.y < max.y)
            {
                selectedObjects.Add(s);
                s.SelectObject();
            }
        }
    }

    private void UpdateSelectionBox(Vector2 position)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = position.x - boxStartPos.x;
        float height = position.y - boxStartPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = 
            boxStartPos + new Vector2(width / 2, height / 2);
    }
}
