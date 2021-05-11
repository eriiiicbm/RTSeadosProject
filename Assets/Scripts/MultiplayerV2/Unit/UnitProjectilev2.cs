using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectilev2 : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int damageToDeal=20;
    [SerializeField] private float launchForce=10f;
    [SerializeField] private float destroyAfterSeconds=5f;
    
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
      Invoke(nameof(DestroySelf),destroyAfterSeconds);
    }
[ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<NetworkIdentity>(out  NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient==connectionToClient)
            {
                return;
            }

            if (other.TryGetComponent<Health>(out Health health))
            {
                health.DealDamage(damageToDeal);
            }
            DestroySelf();
        }
    }

    [ServerCallback]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
