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
    [SerializeField] private LayerMask layerMask = new LayerMask();

    public void detectEnemy(float atackRange)
    {
      Collider[] collider =   Physics.OverlapSphere(this.gameObject.transform.position, atackRange,layerMask);
      if (collider.Length==0)
      {
          return;
      }
      Collider nearCollider = collider[0];
      foreach (var col in collider)
      {
          if (col.GetComponent<RTSBase>().connectionToClient.connectionId ==
              NetworkClient.connection.connectionId) continue;
          if (Vector3.Distance(col.transform.position,this.transform.position) <= Vector3.Distance(nearCollider.transform.position,this.transform.position)  )
          {
              nearCollider = col;
          }
      }
      
      if (nearCollider == null) return;
      
      RTSBase enemy = nearCollider.GetComponent<RTSBase>();
   
      if (enemy == null) return;
      if (enemy.connectionToClient.connectionId ==
          NetworkClient.connection.connectionId)
      {
          return;
      }
      Debug.Log($"{enemy.name} detected");
        GetComponent<ComponentAbility>().SetParameter(rtsEntity.EffectRadious);
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
