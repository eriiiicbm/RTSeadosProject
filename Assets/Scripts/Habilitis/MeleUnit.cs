using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeleUnit : MonoBehaviour, ComponetHability
{
   [Server] public void active(RTSBase target, float damage)
    {
        Debug.Log("damageMele con damage: "+ damage);

        target.DealDamage(damage);
    }
}
