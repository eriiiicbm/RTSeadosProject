using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeleeUnit : MonoBehaviour, ComponentAbility
{
    public void SetParameter(float parameter)
    {Debug.LogError("Can't use this method here");
    }

    [Server] public void active(RTSBase target, float damage)
    {
        Debug.Log("damageMele con damage: "+ damage);

        target.DealDamage(damage);
    }
}
