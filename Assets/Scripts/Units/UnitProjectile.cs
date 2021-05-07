using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;
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
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
