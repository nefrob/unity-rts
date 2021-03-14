using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Selectable : NetworkBehaviour
{   
    public enum SelectType
    {
        UNIT,
        BUILDING
    }

    [SerializeField] protected UnityEvent onSelected = null;
    [SerializeField] protected UnityEvent onDeselected = null;
    [SerializeField] protected SelectType selectableType = SelectType.UNIT;

    public SelectType GetSelectableType()
    {
        return selectableType;
    }

    #region client

    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;
        onDeselected?.Invoke();
    }

    #endregion
}
