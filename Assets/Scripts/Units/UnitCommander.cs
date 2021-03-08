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
        GameOverManager.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverManager.ClientOnGameOver -= ClientHandleGameOver;
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
        } else
        {
            TryMove(hit.point);
        }
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

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

}
