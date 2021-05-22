using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        StartCoroutine(nameof(PickResourcesState));
        StartCoroutine(nameof(BuildState));
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Debug.Log("objetivo: " + target.gameObject.name);

            if (target.gameObject.GetComponent<Building>() != null)
            {
                Building build = target.gameObject.GetComponent<Building>();

                Debug.Log("pasa por aqui i autoridad: " + build.hasAuthority);

                if (build.hasAuthority && build.CurrentHealth != build.MaxHealth)
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

    //[HideInInspector]
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
            Debug.Log("pasa por aqui i faltan "+ (Vector3.Distance(transform.position, resource.transform.position)));

            base.CmdMove(resource.transform.position);
            return;
        }

        Debug.Log("pasa por aqui2");

        currentState = UnitStates.PickResources;
        transform.LookAt(resource.transform.position);
        resource.resourcesQuantity -= 10;
        connectionToClient.identity.GetComponent<RTSPlayerv2>().
            SetResources(10, resource.currentResourceType);
    }

    //[HideInInspector]
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
