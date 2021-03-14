using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionManager : NetworkBehaviour
{
    public HashSet<Selectable> SelectedObjects { get; } = new HashSet<Selectable>();

    [SerializeField] private RectTransform selectionBox = null;
    private Vector2 boxStartPos;

    [SerializeField] private LayerMask selectionMask = new LayerMask();

    private Camera cam; // main camera
    private Player player;
    private Selectable.SelectType curSelectionType;

    public Selectable.SelectType GetCurrentSelectionType()
    {
        return curSelectionType;
    }

    private void Start()
    {
        cam = Camera.main;
        player = NetworkClient.connection.identity.GetComponent<Player>();
        Unit.AuthoryOnUnitDespawn += AuthorityHandleUnitDespawn;
        Building.AuthorityOnBuildingDespawn += AuthorityHandleBuildingDespawn;
        GameOverManager.ClientOnGameOver += ClientHandleGameOver;
        curSelectionType = Selectable.SelectType.UNIT;
    }

    private void OnDestroy()
    {
        Unit.AuthoryOnUnitDespawn -= AuthorityHandleUnitDespawn;
        Building.AuthorityOnBuildingDespawn -= AuthorityHandleBuildingDespawn;
        GameOverManager.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!Keyboard.current.shiftKey.isPressed) 
            {
                DeselectAll();
            }

            boxStartPos = Mouse.current.position.ReadValue();
            selectionBox.gameObject.SetActive(true);

            TrySelect();
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ReleaseSelectionBox();
        }
        if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionBox(); // FIXME: select as update?
        }
    }

    private void TrySelect()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, selectionMask)) return;

        if (!hit.collider.TryGetComponent<Selectable>(out Selectable s)) return;
        if (!s.hasAuthority) return;

        if (Keyboard.current.shiftKey.isPressed)
        {
            if (SelectedObjects.Contains(s))
            {
                SelectedObjects.Remove(s);
                s.Deselect();
                return;
            }
            
            if (s.GetSelectableType() != curSelectionType) return;
        }

        SelectedObjects.Add(s);
        s.Select();
        curSelectionType = s.GetSelectableType();
    }

    private void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        if (selectionBox.sizeDelta.magnitude == 0) return;

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        // FIXME: check if shift still held?

        //? Only units can be box selected
        if (curSelectionType != Selectable.SelectType.UNIT)
        {
            DeselectAll();
        }

        foreach (Unit unit in player.GetAllUnits())
        {
            if (SelectedObjects.Contains((Selectable) unit)) continue;

            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);
            if (screenPos.x > min.x 
                && screenPos.x < max.x 
                && screenPos.y > min.y 
                && screenPos.y < max.y)
            {
                SelectedObjects.Add((Selectable) unit);
                unit.Select();
            }
        }
        
        curSelectionType = Selectable.SelectType.UNIT;
    }

    private void UpdateSelectionBox()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        float width = mousePos.x - boxStartPos.x;
        float height = mousePos.y - boxStartPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = 
            boxStartPos + new Vector2(width / 2, height / 2);
    }

    private void DeselectAll()
    {
        foreach (Selectable s in SelectedObjects)
        {
            s.Deselect();
        }
        SelectedObjects.Clear();
    }

    private void AuthorityHandleUnitDespawn(Unit unit)
    {
        SelectedObjects.Remove((Selectable) unit);
    }

    private void AuthorityHandleBuildingDespawn(Building building)
    {
        SelectedObjects.Remove((Selectable) building);
    }

    private void ClientHandleGameOver(string winnerName)
    {
        this.enabled = false;
    }
}
