using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectilev3 : NetworkBehaviour
{
    public float damage;
    [SerializeField] private float destroyAfterSeconds=5f;
    [SerializeField] public float launchForce=10f;
    [SerializeField] private Rigidbody rb;
    public Transform model;

    public override void OnStartServer()
    {
       // var rotation = model.rotation;
     //   model.rotation  = new Quaternion(rotation.x,rotation.y+180,rotation.z,rotation.w
       // );
        if (rb==null)
        {
            rb = GetComponent<Rigidbody>();
        }
      Invoke(nameof(DestroySelf),destroyAfterSeconds);
      rb.velocity = transform.forward * launchForce;
    //  transform.LookAt(transform.forward);
    }
[ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity)) return;
        Debug.Log($"touched {other.name}");
        if (networkIdentity.connectionToClient==connectionToClient)
        {
            return;
        }
        Debug.Log($"Touched enemy {other.name}");
        if (other.TryGetComponent<RTSBase>(out RTSBase rtsBase))
        {
            
            ComponentAbility componentAbility = GetComponent<ComponentAbility>();
            if (componentAbility!=null)
            {
             Debug.Log("has component");
          
            componentAbility.active(rtsBase.GetComponent<RTSBase>(), damage);

            }
            else
            {
                rtsBase.DealDamage(damage);
                DestroySelf();

            }}

     }

    [ServerCallback]
    private void DestroySelf()
    {
        Debug.Log("proyectile kaboom");
        NetworkServer.Destroy(gameObject);
    }
}
