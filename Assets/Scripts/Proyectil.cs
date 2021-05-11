using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float damage;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<RTSBase>() != null)
        {
            collision.gameObject.GetComponent<RTSBase>().TakeDamage(collision.gameObject.GetComponent<RTSBase>(), damage);
        }
    }
}
