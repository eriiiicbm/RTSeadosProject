using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building, FireBuilding
{
    float atackRange;
    float atackTimer;
    float damege;
    float currentTime;
    float throwForce = 5;
    GameObject proyectils;

    public void detectEnemy(float atackRange)
    {
        Physics.SphereCast(this.gameObject.transform.position, atackRange, transform.forward, out RaycastHit hit);

        if (hit.collider != null) return;
        if (hit.collider.GetComponent<RTSBase>() != null) return;

        RTSBase enemy = hit.collider.GetComponent<RTSBase>();

        fire(hit.collider.gameObject.transform);
    }

    public void fire(Transform currentTarget)
    {
        Vector3 direction = transform.position - currentTarget.position;
        GameObject proyectil = Instantiate(proyectils);
        proyectil.transform.position = transform.position;
        proyectil.transform.Rotate(direction);
        proyectil.GetComponent<Proyectil>().damage = damege;

        proyectil.GetComponent<Rigidbody>().AddForce(direction * throwForce, ForceMode.Impulse);
    }

    // Start is called before the first frame update
    void Start()
    {
        atackRange = rtsEntity.AttackRange;
        atackTimer = rtsEntity.AttackTimer;
        damege = rtsEntity.Damage;
        proyectils = rtsEntity.Proyectile;
        throwForce = rtsEntity.Velocity;
    }

    // Update is called once per frame
    void Update()
    {
        if (rtsEntity.Prefab.transform.Find("TowerEstructure").gameObject.activeInHierarchy)
        {
            if (Time.deltaTime >= currentTime)
            {
                detectEnemy(atackRange);
                currentTime = Time.deltaTime + atackTimer;
            }
        }
    }
}
