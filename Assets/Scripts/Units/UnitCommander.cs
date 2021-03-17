using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommander : MonoBehaviour
{
    [SerializeField] private SelectionManager selectionManager = null;
    [SerializeField] private LayerMask commandMask = new LayerMask();

    private Camera cam; // main camera

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
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, commandMask)) return;

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
        if (selectionManager.GetCurrentSelectionType() !=
            Selectable.SelectType.UNIT) return;

        foreach (Selectable s in selectionManager.SelectedObjects)
        {
            ((Unit) s).GetUnitMovement().CmdMoveUnit(pos);
        }
    }

    private void TryTarget(Targetable target)
    {
        if (selectionManager.GetCurrentSelectionType() !=
            Selectable.SelectType.UNIT) return;

        foreach (Selectable s in selectionManager.SelectedObjects)
        {
            ((Unit) s).GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false; // disable commands
    }
}
