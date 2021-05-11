using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeHouse : Building, Fridege
{
    float efectRadius;
    float recoverVelocity;
    bool inTrigger;
    List<Unit> units = new List<Unit>();
    public void recoverUnits()
    {
        foreach (var unit in units) {
            if(unit.MaxHealth>unit.CurrentHealth)
            unit.CurrentHealth++;
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

}
