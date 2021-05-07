using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeHouse : Building, Fridege
{
    float efectRadius;
    float recoverVelocity;
    bool inTrigger;
    List<RTSBase> units = new List<RTSBase>();
    public void recoverUnits()
    {
        foreach (var unit in units) {
            if(unit.MaxHealth>unit.Health)
            unit.Health++;
        }    }

    // Start is called before the first frame update
      private void Start()
    {

        base.Start();
        efectRadius = rtsEntity.EffectRadious;   
        recoverVelocity = rtsEntity.RecoverySpeed;        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    private void OnTriggerEnter(Collider other)
    {
        RTSBase rtsaBase = other.GetComponent<RTSBase>();
        units.Add(rtsaBase);
    }
    private void OnTriggerStay(Collider other)
    {
        recoverUnits();

    }
    private void OnTriggerExit(Collider other)
    {
        RTSBase rtsaBase = other.GetComponent<RTSBase>();
        units.Remove(rtsaBase);

    }

}
