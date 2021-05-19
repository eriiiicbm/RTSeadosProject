using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoralSupport : PasiveHability
{
    public override void PasiveEffect(Unit unit)
    {
        if (GetComponent<RTSBase>().connectionToClient != unit.connectionToClient) return;

        unit.DealMoralSupport(recoverySpeed);
    }
    /*private void OnTriggerStay(Collider other)
    {
        recoverUnits();
    }*/
}
