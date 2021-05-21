using System.Collections;
using System.Collections.Generic;
using Mirror;
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
 [ServerCallback]
    void Update()
    {
        base.Update();
        attackSpeed += Time.deltaTime;

        if (target != null)
        {
            Vector3 pos = target.transform.position;
            navMeshAgent.destination = pos;

            var distance = (transform.position - pos).magnitude;
            Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 180 * Time.deltaTime);

            if (!(distance <= attackDistance) || !(attackSpeed >= attackTimer)) return;
            Debug.Log("ESTA PEGANDO ");
            GetComponent<ComponetHability>().active(target.GetComponent<RTSBase>(), damage);

            attackSpeed = 0;
        }
    }
}