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

    public void detectEnemy(float atackRange)
    {
        Physics.SphereCast(this.gameObject.transform.position, atackRange, transform.forward, out RaycastHit hit);
 
        if (hit.collider == null) return;
        if (hit.collider.GetComponent<RTSBase>() != null) return;


        RTSBase enemy = hit.collider.GetComponent<RTSBase>();
Debug.Log($"{enemy.name} detected");
        GetComponent<ComponentAbility>().active(enemy, damege);
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
        if (  !(attackSpeed >= attackTimer)) return;
        Debug.Log("Time to attack");

        detectEnemy(attackRange);
         attackSpeed = 0;

    }

    private void OnDrawGizmosSelected()
    {
Gizmos.DrawSphere(this.gameObject.transform.position, attackRange);    }
}
