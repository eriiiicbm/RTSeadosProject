using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DistanceUnit : NetworkBehaviour, ComponentAbility
{
    private GameObject projectils;
    public Transform spawnProyectilPoint;
    private RTSBase rtsBase;
    private void Start()
    {
          rtsBase = GetComponent<RTSBase>();
        
        projectils = rtsBase.rtsEntity.Proyectile;
    }

    public void SetParameter(float parameter)
      {Debug.LogError("Can't use this method here");
    }

    [Server]
    public void active(RTSBase target, float damage)
    {
        Debug.Log("damageDistance con damage: " + damage);
        if (target==null)
        {
            Debug.LogError("Target of the damage is null");
            return;
        }
        Vector3 direction = transform.position - target.transform.position;
        Transform aimPoint = target.GetComponent<Targetable>().GetAimPoint();
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position-transform.position);
        transform.rotation= Quaternion.RotateTowards(transform.rotation,targetRotation,180f*Time.deltaTime);
        Quaternion projectileRotation =
            Quaternion.LookRotation(target.transform.position - spawnProyectilPoint.position);
        //projectileRotation.y += 90;
        GameObject projectile = Instantiate(projectils,spawnProyectilPoint.position,projectileRotation );
     projectile.GetComponent<ComponentAbility>()?.SetParameter(rtsBase.rtsEntity.EffectRadious);
        NetworkServer.Spawn(projectile,connectionToClient);
        projectile.name = "THE POTATO";
        //projectile.transform.position = spawnProyectilPoint.position;
     //   projectile.transform.Rotate(direction);
            UnitProjectilev3 unitProjectilev3 = 
                projectile.GetComponent<UnitProjectilev3>();
            unitProjectilev3.damage = damage;
            unitProjectilev3.launchForce = 10;

       // projectile.GetComponent<Rigidbody>().AddForce(direction * 100f, ForceMode.Impulse);
    }
}
