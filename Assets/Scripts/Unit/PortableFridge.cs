using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PortableFridge : Unit
{
    float efectRadius;
    float recoverVelocity;
    bool inTrigger;
    List<Unit> units = new List<Unit>();
   [Command]
    public void recoverUnits()
    {
        foreach (var unit in units)
        {
            if (unit.MaxHealth > unit.CurrentHealth)
              DealDamage(-recoverVelocity);
            StartCoroutine(Wait(0.5f));
        }
    }

    // Start is called before the first frame update
    private void Start()
    {

        efectRadius = rtsEntity.EffectRadious;
        recoverVelocity = rtsEntity.RecoverySpeed;
    }

    // Update is called once per frame
  [ServerCallback]
    void Update()
    {
    }
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        Unit rtsaBase = other.GetComponent<Unit>();
        units.Add(rtsaBase);
    }
    [Server]
    private void OnTriggerStay(Collider other)
    {
        recoverUnits();

    }
    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        Unit rtsaBase = other.GetComponent<Unit>();
        units.Remove(rtsaBase);

    }

    IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

}
