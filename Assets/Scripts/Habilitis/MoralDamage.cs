using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoralDamage : PasiveHability
{
    public float damageMoral;

    public override void PasiveEffect()
    {
        foreach (var unit in units)
        {
            unit.DealMoralDamage(damageMoral);
            StartCoroutine(Wait(0.5f));
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        damageMoral = GetComponent<Unit>().rtsEntity.DamageMoral;
    }
    /*private void OnTriggerStay(Collider other)
    {
        damageMoralUnits();
    }*/
}
