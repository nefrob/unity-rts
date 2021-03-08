using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform targetPoint = null;

    public Transform GetTargetPoint()
    {
        return targetPoint;
    }
}
