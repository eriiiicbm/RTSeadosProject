using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectilev3 : NetworkBehaviour
{
    public float damage;
    [SerializeField] private float destroyAfterSeconds=5f;
    

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

            if (other.TryGetComponent<RTSBase>(out RTSBase rtsBase))
            {
                rtsBase.DealDamage(damage);
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
