using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Unit
{
    ResourcesType currentResourceType;
    int resourceQuantity=0;
    float buildRate;
    float pickRate;
    float range;

    void Start()
    {
 
        buildRate = rtsEntity.AttackTimer;
        range = rtsEntity.AttackRange;

        InvokeRepeating("build", buildRate, buildRate);
        InvokeRepeating("recolect", pickRate, pickRate);
    }

    // Update is called once per frame
    void Update()
    {

       
    }

    [HideInInspector]
    public Resource resource;
    public void recolect() {
        if (resource == null)
            return;
        if (Vector3.Distance(transform.position, resource.transform.position) > range)
        {
            base.CmdMove(resource.transform.position);
            return;
        }
        transform.LookAt(resource.transform.position);
        resource.resourcesQuantity -= 10;
        //CivilizationMetrics.singleton[movileEntity.entity.faction].resources += 10;
    }

    [HideInInspector]
    public Building building;
    public void build()
    {
        if (building == null)
            return;
        if (Vector3.Distance(transform.position, building.transform.position) > range)
        {
            base.CmdMove(building.transform.position);
            currentState = UnitStates.Building;
            return;
        }
        if (building.buildTime <= 0)
        {
            building = null;
            return;
        }
        transform.LookAt(building.transform.position);

        building.SendMessage("CraftPoint");
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
    public IEnumerator BuildState()
    {
        while (currentState == UnitStates.Building)
        {

            yield return 0;

        }
        yield return new WaitForEndOfFrame();

    }
}