using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceUnit : MonoBehaviour, ComponetHability
{
    private GameObject projectils;

    private void Start()
    {
        RTSBase rtsBase = GetComponent<RTSBase>();
        
        projectils = rtsBase.rtsEntity.Proyectile;
    }

    public void active(RTSBase target, float damage)
    {
        Vector3 direction = transform.position - target.transform.position;
        GameObject proyectil = Instantiate(projectils);
        proyectil.transform.position = transform.position;
        proyectil.transform.Rotate(direction);
        proyectil.GetComponent<UnitProjectilev3>().damage = damage;

        proyectil.GetComponent<Rigidbody>().AddForce(direction * 10f, ForceMode.Impulse);
    }
}
