using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : Unit
{
    float damage;
    float attackDistance;
    float attackSpeed;
    // Start is called before the first frame update
    void Start()
    {
        attackSpeed = rtsEntity.AttackTimer;

    }

    // Update is called once per frame
    void Update()
    {
       // base.Update();

        attackSpeed += Time.deltaTime;
        if (currentTarget != null)
        {
            navAgent.destination = currentTarget.position;

            var distance = (transform.position - currentTarget.position).magnitude;

            if (distance <= rtsEntity.AttackRange)
            {
                Attack();
            }
        }
    }
    void Attack()
    {
        if (attackSpeed >= rtsEntity.AttackTimer)
        {
            RTSFoodManager.UnitTakeDamage(this, currentTarget.GetComponent<UnitCombat>());
            attackSpeed = 0;

        }
    }
}
