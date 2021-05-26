using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Fridge : PasiveHability
{
    [Server]
    public override void PasiveEffect(Unit unit)
    {
        if (GetComponent<RTSBase>().connectionToClient != unit.connectionToClient) return;
    
        Debug.Log(unit.name+" unidad recuperada");
        unit.DealDamage(-1 * recoverySpeed);        
    }
    /*private void OnTriggerStay(Collider other)
    {
        recoverUnits();
    }*/
}