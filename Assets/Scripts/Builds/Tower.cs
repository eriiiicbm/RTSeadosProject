using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : Building
{
    float atackRange;
    float atackTimer;
    float damege;
    float currentTime;
    float throwForce = 5;
    GameObject proyectils;

    public void detectEnemy(float atackRange)
    {
        Physics.SphereCast(this.gameObject.transform.position, atackRange, transform.forward, out RaycastHit hit);

        if (hit.collider != null) return;
        if (hit.collider.GetComponent<RTSBase>() != null) return;


        RTSBase enemy = hit.collider.GetComponent<RTSBase>();

        GetComponent<ComponetHability>().active(enemy, damege);
    }

    // Start is called before the first frame update
    void Start()
    {
        atackRange = rtsEntity.AttackRange;
        atackTimer = rtsEntity.AttackTimer;
        damege = rtsEntity.Damage;
        proyectils = rtsEntity.Proyectile;
        throwForce = rtsEntity.Velocity;

        StartCoroutine(nameof(Builded));
    }

    // Update is called once per frame
    void Update()
    {
        /*if (transform.Find("FinalEstructure").gameObject.activeInHierarchy)
        {
            if (Time.deltaTime >= currentTime)
            {
                detectEnemy(atackRange);
                currentTime = Time.deltaTime + atackTimer;
            }
        }*/
    }

    public IEnumerator Builded()
    {
        while (base.builded)
        {
            if (Time.deltaTime >= currentTime)
            {
                detectEnemy(atackRange);
                currentTime = Time.deltaTime + atackTimer;
            }

            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }
}
