using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : Unit
{
    float buildRate;
    float range;

    public override void OnStartServer()
    {
        base.OnStartServer();
        buildRate = rtsEntity.AttackTimer;
        range = rtsEntity.AttackRange;

        InvokeRepeating("build", buildRate, buildRate);
        InvokeRepeating("recolect", buildRate, buildRate);
    }

    public  override  void Update()
    {
        base.Update();
        if (target != null)
        {
            base.Update();

            if(target.gameObject.GetComponent<Building>() != null)
            {
                Building build= target.gameObject.GetComponent<Building>();
                if (build.CurrentHealth != build.MaxHealth)
                {
                    building = build;
                }
            }

            if(target.gameObject.GetComponent<Resource>() != null)
            {
                resource = target.gameObject.GetComponent<Resource>();
            }
        }
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
        currentState = UnitStates.PickResources;
        transform.LookAt(resource.transform.position);
        resource.resourcesQuantity -= 10;
        connectionToClient.identity.GetComponent<RTSPlayerv2>().
            SetResources(10, resource.currentResourceType);
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
            return;
        }
        if (building.buildTime <= 0)
        {
            building = null;
            return;
        }
        currentState = UnitStates.Building;
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
        GoToNextState();
        
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
