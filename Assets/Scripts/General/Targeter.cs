using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targeter : NetworkBehaviour
{
    private Targetable target;

    public Targetable GetTarget()
    {
        return target;
    }

    #region server

    [Command]
    public void CmdSetTarget(GameObject targetObject)
    {
        if (!targetObject.TryGetComponent<Targetable>(out Targetable target)) return;

        this.target = target;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion
}
