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

    // Start is called before the first frame update
    private void Start()
    {
        recoverySpeed = GetComponent<RTSEntity>().RecoverySpeed;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void OnTriggerStay(Collider other)
    {
        recoverUnits();
    }
}
