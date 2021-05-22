using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeHouse : Building
{
    float efectRadius;
    float recoverVelocity;
    bool inTrigger;
    List<Unit> units = new List<Unit>();
    public void recoverUnits()
    {
        foreach (var unit in units) {
            if(unit.MaxHealth>unit.CurrentHealth)
            unit.DealDamage(-recoverVelocity);
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
    void Update()
    {
    }
    private void OnTriggerEnter(Collider other)
    {
        Unit rtsaBase = other.GetComponent<Unit>();
        units.Add(rtsaBase);
    }
    private void OnTriggerStay(Collider other)
    {
        recoverUnits();

    }
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
