using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class DistanceUnit : MonoBehaviour, ComponetHability
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
        GameObject proyectil = Instantiate(projectils);
        proyectil.transform.position = spawnProyectilPoint.position;
        proyectil.transform.Rotate(direction);
        proyectil.GetComponent<UnitProjectilev3>().damage = damage;

        proyectil.GetComponent<Rigidbody>().AddForce(direction * 100f, ForceMode.Impulse);
    }
}
