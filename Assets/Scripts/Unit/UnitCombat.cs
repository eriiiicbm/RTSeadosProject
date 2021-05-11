using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCombat : Unit
{
    public float damage;
    float attackDistance;
    float attackSpeed;
    float attackTimer;
    // Start is called before the first frame update
    void Start()
    {
        attackSpeed = rtsEntity.AttackTimer;
        attackDistance = rtsEntity.AttackRange;
        damage = rtsEntity.Damage;
        attackTimer = rtsEntity.AttackTimer;
    }

    // Update is called once per frame
    void Update()
    {
       // base.Update();

        attackSpeed += Time.deltaTime;
        if (currentTarget != null)
        {
            navMeshAgent.destination = currentTarget.position;

            var distance = (transform.position - currentTarget.position).magnitude;

            if (distance <= attackDistance)
            {
                Attack();
            }
        }
    }
    void Attack()
    {
        if (attackSpeed >= attackTimer)
        {
            RTSFoodManager.UnitTakeDamage(this, currentTarget.GetComponent<UnitCombat>());
            attackSpeed = 0;

        }
    }
}
