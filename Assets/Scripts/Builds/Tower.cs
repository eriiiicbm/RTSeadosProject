using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building, FireBuilding
{
    float atackRange;
    float atackTimer;
    float currentTime;
    float nextFire;
    float throwForce = 5;

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
        GetComponent<Rigidbody>().AddForce(transform.position - currentTarget.position * throwForce);
    }

    // Start is called before the first frame update
    void Start()
    {
        atackRange = rtsEntity.AttackRange;
        atackTimer = rtsEntity.AttackTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.deltaTime >= currentTime)
        {
            detectEnemy(atackRange);
            currentTime = Time.deltaTime + atackTimer;
        }
    }
}
