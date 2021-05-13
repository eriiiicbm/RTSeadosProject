using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : PasiveHability
{
    public void recoverUnits()
    {
        foreach (var unit in units)
        {
            if (unit.MaxHealth > unit.CurrentHealth)
                unit.CurrentHealth += recoverySpeed;
            StartCoroutine(Wait(0.5f));
        }
    }
    private void OnTriggerStay(Collider other)
    {
        recoverUnits();
    }
}