using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<RTSBase>() != null)
        {
            collision.gameObject.GetComponent<RTSBase>().TakeDamage(collision.gameObject.GetComponent<RTSBase>(), damage);
        }
    }
}
