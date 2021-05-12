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

        attackSpeed += Time.deltaTime;
        if (currentTarget != null)
        {
            navMeshAgent.destination = currentTarget.position;

            var distance = (transform.position - currentTarget.position).magnitude;
            Quaternion targetRotation = Quaternion.LookRotation(currentTarget.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime);

            if (distance <= attackDistance && attackSpeed >= attackTimer)
            {
                GetComponent<ComponetHability>().active(currentTarget.GetComponent<RTSBase>(), damage);

                attackSpeed = 0;
            }
        }
    }
}
