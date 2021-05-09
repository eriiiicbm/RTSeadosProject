using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimAtPoint;

    public Transform GetAimPoint()
    {
        return aimAtPoint;
    }

   
}
