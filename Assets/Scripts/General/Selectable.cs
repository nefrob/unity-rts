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

    private bool isSelected;

    public SelectType GetSelectableType()
    {
        return selectableType;
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    #region client

    [ClientCallback]
    private void Start()
    {
        isSelected = false;
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority) return;
        isSelected = true;
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority) return;
        isSelected = false;
        onDeselected?.Invoke();
    }

    #endregion
}
