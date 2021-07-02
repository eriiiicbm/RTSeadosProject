using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Mirror;

public class Villager : Unit
{
   [SyncVar] float buildRate;
   [SyncVar] float range;
   [SerializeField] private AudioClip recolectSound;
    public override void OnStartServer()
    {
        base.OnStartServer();
        buildRate = rtsEntity.AttackTimer;
        range = rtsEntity.AttackRange;

        InvokeRepeating(nameof(Build), buildRate, buildRate);
        InvokeRepeating(nameof(Recolect), buildRate, buildRate);
        StartCoroutine(nameof(PickResourcesState));
  //      StartCoroutine(nameof(BuildState));
    }

 

    [HideInInspector]
    public Resource resource;
    public void Recolect() {
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
            unitStates = UnitStates.Idle;
            return;
        }
        int resourceCatch = 10;

        StopCoroutine(nameof(PickageAnim));
        StartCoroutine(nameof(PickageAnim));
        Debug.Log("state: " + unitStates);
        transform.LookAt(resource.transform.position);
        SoundManager._instance.PlaySE(recolectSound,1f);
        resource.resourcesQuantity -= resourceCatch;
        RTSPlayerv2 player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
        player.SetResources(player.GetResources(resource.currentResourceType) + resourceCatch, resource.currentResourceType);
    }

    [HideInInspector]
    public Building building;
    public void Build()
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
            unitStates = UnitStates.Idle;
            return;
        }
  
        transform.LookAt(building.transform.position);
        unitStates = UnitStates.Attack;
        StopCoroutine(nameof(PickageAnim));
        StartCoroutine(nameof(PickageAnim));
        building.SendMessage("CraftPoint");
    }

    public void VillagerStuff()
    {
      

        if (target == null) return;
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
    public override IEnumerator IdleState()
    { 
        while (unitStates== UnitStates.Idle) {
            VillagerStuff();


            yield return new WaitForEndOfFrame();
        }
        GoToNextState();
        yield return new WaitForEndOfFrame();
    }

    public override IEnumerator WalkState()
    {
        StartCoroutine((base.WalkState()));
         
        while (unitStates== UnitStates.Walk) {
            VillagerStuff();


            yield return new WaitForEndOfFrame();
        } 

    }

    public IEnumerator PickResourcesState() {
        while (unitStates == UnitStates.PickResources)
        {
          
            yield return new WaitForEndOfFrame();

        }
        yield return new WaitForEndOfFrame();
        GoToNextState();
        
    }
    public IEnumerator BuildingState()
    {
        while (unitStates == UnitStates.Building)
        {

            yield return new WaitForEndOfFrame();

        }
        yield return new WaitForEndOfFrame();

    }
    [ContextMenu("Deal damage")]
    public void TestDealDamage()
    {
     DealDamage(500);   
    }
    
    IEnumerator PickageAnim()
    
    {
        unitStates = UnitStates.Attack;
animator.Play("Attack");
        yield return new WaitForEndOfFrame();
        
        if (animator==null)
        {
            yield break;
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
 
        
    }
}
