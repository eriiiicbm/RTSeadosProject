using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Tower : Building
{
    float attackRange;
    float attackTimer;
    float damege;
    float attackSpeed;
    float throwForce = 5;
    GameObject proyectils;
    [SerializeField] GameObject towerCannon;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Transform lookAt;

    public void DetectEnemy(float attackRange)
    {
        Collider[] collider = Physics.OverlapSphere(this.gameObject.transform.position, attackRange, layerMask);
        if (collider.Length == 0)
        {
            return;
        }

        Collider nearCollider = collider[0];
        foreach (var col in collider)
        {
            if (col.GetComponent<RTSBase>().connectionToClient == null)
            {
                continue;
            }

            if (col.GetComponent<RTSBase>().connectionToClient.connectionId ==
                NetworkClient.connection.connectionId) continue;
            if (Vector3.Distance(col.transform.position, this.transform.position) <=
                Vector3.Distance(nearCollider.transform.position, this.transform.position))
            {
                nearCollider = col;
            }
        }

        if (nearCollider == null) return;

        RTSBase enemy = nearCollider.GetComponent<RTSBase>();

        if (enemy == null) return;
        if (enemy.connectionToClient == null)
        {
            return;
        }

        if (enemy.connectionToClient.connectionId ==
          connectionToClient.connectionId)
        {
            return;
        }

        transform.LookAt(enemy.transform.position);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        eulerAngles.y -= 180;

        transform.rotation = Quaternion.Euler(eulerAngles);

        if (!(attackSpeed >= attackTimer)) return;
        Debug.Log("Time to attack");
        Debug.Log($"{enemy.name} detected");
        GetComponent<ComponentAbility>().SetParameter(rtsEntity.EffectRadious);
        GetComponent<ComponentAbility>().active(enemy, damege);
        attackSpeed = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        attackRange = rtsEntity.AttackRange;
        attackTimer = rtsEntity.AttackTimer;
        attackSpeed = rtsEntity.AttackTimer;
        damege = rtsEntity.Damage;
        proyectils = rtsEntity.Proyectile;
        throwForce = rtsEntity.Velocity;


        attackTimer = rtsEntity.AttackTimer;
    }

    // Update is called once per frame
    [ServerCallback]
    void Update()
    {
        attackSpeed += Time.deltaTime;

        //     Debug.Log($"{attackSpeed} attackspeed");

        if (!base.builded) return;
        Debug.Log("Tower is builded");

        DetectEnemy(attackRange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(this.gameObject.transform.position, attackRange);
    }
}