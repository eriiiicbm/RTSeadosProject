using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MoralDamage : PassiveAbility
{
    public float damageMoral;

    [Server]public override void PasiveEffect(Unit unit)
    {
        if (GetComponent<RTSBase>().connectionToClient == unit.connectionToClient) return;

        unit.DealMoralDamage(damageMoral);
    }

    // Start is called before the first frame update
    private void Start()
    {
        damageMoral = GetComponent<Unit>().rtsEntity.DamageMoral;
    }
}
