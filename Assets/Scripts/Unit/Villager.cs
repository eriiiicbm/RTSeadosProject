using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Villager : Unit
{
   [SyncVar] float buildRate;
   [SyncVar] float range;

    public override void OnStartServer()
    {
        base.OnStartServer();
        buildRate = rtsEntity.AttackTimer;
        range = rtsEntity.AttackRange;

        InvokeRepeating("build", buildRate, buildRate);
        InvokeRepeating("recolect", buildRate, buildRate);
        StartCoroutine(nameof(PickResourcesState));
        StartCoroutine(nameof(BuildState));
    }

   [ServerCallback]
    private void FixedUpdate()
    {
        if (target != null)
        {
            Debug.Log("objetivo: " + target.gameObject.name);

            if (target.gameObject.GetComponent<Building>() != null)
            {
                Building build = target.gameObject.GetComponent<Building>();

                Debug.Log("pasa por aqui i autoridad: " + build.hasAuthority);

                if (build.connectionToClient.connectionId == connectionToClient.connectionId && build.CurrentHealth != build.MaxHealth)
                {
                    building = build;
                    resource = null;
                    return;
                }
            }

            if (target.gameObject.GetComponent<Resource>() != null)
            {
                resource = target.gameObject.GetComponent<Resource>();
                building = null;
                return;
            }

            resource = null;
            building = null;
        }

        
    }

    [HideInInspector]
    public Resource resource;
    public void recolect() {
        if (resource == null)
            return;

        Vector3 currentPosition = transform.position, targetPosition = resource.transform.position;

        float distance = Mathf.Sqrt(
            Mathf.Pow(currentPosition.x - targetPosition.x, 2f) +
            Mathf.Pow(currentPosition.z - targetPosition.z, 2f));

        if (distance > range)
        {

            base.CmdMove(resource.transform.position);
            return;
        }
        if (resource.resourcesQuantity<= 0)
        {
            resource = null;
            currentState = UnitStates.Idle;
            return;
        }
        int resourceCatch = 10;

        currentState = UnitStates.PickResources;
        transform.LookAt(resource.transform.position);
        resource.resourcesQuantity -= resourceCatch;
        RTSPlayerv2 player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
        player.SetResources(player.GetResources(resource.currentResourceType) + resourceCatch, resource.currentResourceType);
    }

    //[HideInInspector]
    public Building building;
    public void build()
    {
        if (building == null)
            return;

        Vector3 currentPosition = transform.position, targetPosition = building.transform.position;

        float distance = Mathf.Sqrt(
            Mathf.Pow(currentPosition.x - targetPosition.x, 2f) +
            Mathf.Pow(currentPosition.z - targetPosition.z, 2f));

        if (distance > range) { 
            base.CmdMove(building.transform.position);
            return;
        }
        if (building.CurrentHealth >= building.MaxHealth)
        {
            building = null;
            currentState = UnitStates.Idle;
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
    [ContextMenu("Deal damage")]
    public void TestDealDamage()
    {
     DealDamage(500);   
    }
}
