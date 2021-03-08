using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MeleeAttack : Attack
{
    #region server

    [ServerCallback]
    protected override void DoAttack()
    {
        // TODO: perform melee attack
    }

    #endregion
}
