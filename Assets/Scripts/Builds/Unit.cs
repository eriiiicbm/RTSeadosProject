using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : RTSBase
{

    public Transform currentTarget;
    public NavMeshAgent navAgent;
    public float expirationVelocity;
    public int[] prices;
    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    public void Update()
    {

        if (currentTarget != null)
        {
            navAgent.destination = currentTarget.position;

            var distance = (transform.position - currentTarget.position).magnitude;

        }
    }

    void MoveUnit(Vector3 dest)
    {
        currentTarget = null;
        navAgent.destination = dest;
    }
    void SetNewTarget(Transform target)
    {
        currentTarget = target;

    }
    void ExpirationEffect()
    {
        Destroy(this.transform.parent.gameObject);
    }
}
