using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DistanceUnit : NetworkBehaviour, ComponentAbility
{
    private GameObject projectils;
    public Transform spawnProyectilPoint;

    private void Start()
    {
        RTSBase rtsBase = GetComponent<RTSBase>();
        
        projectils = rtsBase.rtsEntity.Proyectile;
    }
[Server]
    public void active(RTSBase target, float damage)
    {
        Debug.Log("damageDistance con damage: " + damage);

        Vector3 direction = transform.position - target.transform.position;
        Quaternion projectileRotation =
            Quaternion.LookRotation(target.GetComponent<Targetable>().GetAimPoint().position - projectils.transform.position);
        GameObject projectile = Instantiate(projectils,spawnProyectilPoint.position,projectileRotation);
     
        NetworkServer.Spawn(projectile,connectionToClient);
        projectile.name = "THE POTATO";
        //projectile.transform.position = spawnProyectilPoint.position;
        projectile.transform.Rotate(direction);
            UnitProjectilev3 unitProjectilev3 = 
                projectile.GetComponent<UnitProjectilev3>();
            unitProjectilev3.damage = damage;
            unitProjectilev3.launchForce = 10;

       // projectile.GetComponent<Rigidbody>().AddForce(direction * 100f, ForceMode.Impulse);
    }
}
