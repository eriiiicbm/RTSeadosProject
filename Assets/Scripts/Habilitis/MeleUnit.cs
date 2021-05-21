using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeleUnit : MonoBehaviour, ComponetHability
{
   [Server] public void active(RTSBase target, float damage)
    {
        target.DealDamage(damage);
    }
}
