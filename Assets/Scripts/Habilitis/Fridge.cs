using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : PasiveHability
{
    public override void PasiveEffect()
    {
        foreach (var unit in units)
        {
            if (unit.MaxHealth > unit.CurrentHealth)
            {
                Debug.Log(unit.name+" unidad recuperada");
                unit.DealDamage(-1 * recoverySpeed);
            }

            StartCoroutine(Wait(0.5f));
        }
    }
    /*private void OnTriggerStay(Collider other)
    {
        recoverUnits();
    }*/
}