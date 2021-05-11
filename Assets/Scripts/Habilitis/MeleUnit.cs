using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleUnit : MonoBehaviour, ComponetHability
{
    public void active(RTSBase target, float damage)
    {
        target.TakeDamage(target, damage);
    }
}
