using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitFiringv2 : NetworkBehaviour
{
    [SerializeField] private Targeter targeter;
    [SerializeField] private GameObject proyectilePrefab;
    [SerializeField] private Transform proyectileSpawnPoint;

    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;
     private float lastFireTime  = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    #region Server
[ServerCallback]
    void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target==null)
        {
            return;
        }
        if (!CanFireATarget())
        {
            return;
        }
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position-transform.position);
        transform.rotation= Quaternion.RotateTowards(transform.rotation,targetRotation,rotationSpeed*Time.deltaTime);
        if (Time.time>(1/fireRate)+lastFireTime)
        {
            Quaternion projectileRotation =
                Quaternion.LookRotation(target.GetAimPoint().position - proyectileSpawnPoint.position);
            GameObject projectileInstance = Instantiate(proyectilePrefab, proyectileSpawnPoint.position,projectileRotation);
            NetworkServer.Spawn(projectileInstance,connectionToClient);
            lastFireTime = Time.time;
        }
    }

    private bool CanFireATarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }

    #endregion  
  
}
