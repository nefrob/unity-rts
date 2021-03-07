using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommander : MonoBehaviour
{
    [SerializeField] private UnitSelectionManager unitSelectionManager = null;

    [SerializeField] private LayerMask targetMask = new LayerMask();

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;
        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, targetMask)) return;

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.hasAuthority) 
            {
                TryMove(hit.point);
            } else 
            {
                TryTarget(target);
            }

            return;
        }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 pos)
    {
        foreach (Unit u in unitSelectionManager.SelectedUnits)
        {
            u.GetUnitMovement().CmdMoveUnit(pos);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionManager.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }
}
