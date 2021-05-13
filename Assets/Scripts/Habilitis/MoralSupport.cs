using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoralSupport : PasiveHability
{
    public void recoverUnits()
    {
        foreach (var unit in units)
        {
            unit.DealMoralSupport(recoverySpeed);
            StartCoroutine(Wait(0.5f));
        }
    }
    private void OnTriggerStay(Collider other)
    {
        recoverUnits();
    }
}
