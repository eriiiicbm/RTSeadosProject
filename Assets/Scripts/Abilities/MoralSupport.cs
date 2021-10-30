using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MoralSupport : PassiveAbility
{
  [Server]  public override void PasiveEffect(Unit unit)
    {
        if (GetComponent<RTSBase>().connectionToClient != unit.connectionToClient) return;

        unit.DealMoralSupport(recoverySpeed);
    }
}
