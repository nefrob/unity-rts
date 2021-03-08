using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;

public class Selectable : NetworkBehaviour
{   
    [SerializeField] protected UnityEvent onSelected = null;
    [SerializeField] protected UnityEvent onDeselected = null;

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
