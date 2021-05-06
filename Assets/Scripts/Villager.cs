using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : RTSBase
{
    ResourcesType currentResourceType;
    int resourceQuantity=0;
    //
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

       
    }
    public void Recolect() {
       

    }
    public override IEnumerator IdleState()
    {
        while (currentState== UnitStates.Idle) {
            yield return 0;


        }
        yield return new WaitForEndOfFrame();
    }
    public IEnumerator PickResourcesState() {
        while (currentState == UnitStates.PickResources)
        {

            yield return 0;

        }
        yield return new WaitForEndOfFrame();

    }
}
