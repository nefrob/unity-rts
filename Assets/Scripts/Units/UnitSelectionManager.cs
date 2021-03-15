using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class UnitSelectionManager : MonoBehaviour
{
    // TODO: delete script

    public HashSet<Unit> SelectedUnits { get; } = new HashSet<Unit>();

    [SerializeField] private RectTransform selectionBox = null;
    private Vector2 boxStartPos;

    [SerializeField] private LayerMask selectionMask = new LayerMask();

    private Camera cam; // main camera
    private Player player;

    private void Start()
    {
        cam = Camera.main;
        player = NetworkClient.connection.identity.GetComponent<Player>();
        Unit.AuthoryOnUnitDespawn += AuthorityHandleUnitDespawn;
        GameOverManager.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthoryOnUnitDespawn -= AuthorityHandleUnitDespawn;
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

        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
        if (!unit.hasAuthority) return;

        if (Keyboard.current.shiftKey.isPressed && SelectedUnits.Contains(unit))
        {
            SelectedUnits.Remove(unit);
            unit.Deselect();
            return;
        }

        SelectedUnits.Add(unit);
        unit.Select();
    }

    private void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        if (selectionBox.sizeDelta.magnitude == 0) return;

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        // FIXME: check if shift still held?

        foreach (Unit unit in player.GetAllUnits())
        {
            if (SelectedUnits.Contains(unit)) continue;

            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);
            
            if (screenPos.x > min.x 
                && screenPos.x < max.x 
                && screenPos.y > min.y 
                && screenPos.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
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
        foreach (Unit u in SelectedUnits)
        {
            u.Deselect();
        }
        SelectedUnits.Clear();
    }

    private void AuthorityHandleUnitDespawn(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string winnerName)
    {
        this.enabled = false;
    }
}
