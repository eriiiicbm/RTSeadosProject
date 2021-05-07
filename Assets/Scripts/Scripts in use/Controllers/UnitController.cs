using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour {

    private NavMeshAgent navAgent;
    private Transform currentTarget;
    private float attackTimer;
    private float unitHealth;

    private Color defaultMaterial;
    public UnitStats unitStats;

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        attackTimer = unitStats.attackSpeed;
        unitHealth = unitStats.health;
        defaultMaterial = GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        attackTimer += Time.deltaTime;
        if(currentTarget != null)
        {
            navAgent.destination = currentTarget.position;

            var distance = (transform.position - currentTarget.position).magnitude;

            if(distance <= unitStats.attackRange)
            {
                Attack();
            }
        }
    }

    public void MoveUnit(Vector3 dest)
    {
        currentTarget = null;
        navAgent.destination = dest;
    }

    public void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);
    }

    public void SetNewTarget(Transform enemy)
    {
        currentTarget = enemy;
    }

    public void Attack()
    {
        if(attackTimer >= unitStats.attackSpeed)
        {
            RTSGameManager.UnitTakeDamage(this, currentTarget.GetComponent<UnitController>());
            attackTimer = 0;
        }
        
    }

    public void TakeDamage(UnitController enemy, float damage)
    {
        unitHealth = unitHealth - damage;
        StartCoroutine(Flasher());
    }

    IEnumerator Flasher()
    {
        var renderer = GetComponent<Renderer>();
        if (unitHealth != 0)
        {
            for (int i = 0; i < 2; i++)
            {
                renderer.material.color = Color.gray;
                yield return new WaitForSeconds(.05f);
                renderer.material.color = defaultMaterial;
                yield return new WaitForSeconds(.05f);

            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
