using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
 
public class AreaDamage : NetworkBehaviour, ComponentAbility
{   
    [SerializeField] private LayerMask layerMask =~8;

    private float effectRadious=20f;
    private void SetEffectRadious(float newEffectRadious)
    {
      effectRadious= newEffectRadious;
    }

    [Server]
    public void active(RTSBase target, float damage)
    {
        Debug.Log("damageArea con damage: " + damage);
        Collider[] hits =   Physics.OverlapSphere(this.gameObject.transform.position,effectRadious ,layerMask);
        foreach (var col in hits)
        {
            Debug.Log(col.name + " name ");
            RTSBase rtsBase = col.GetComponent<RTSBase>();
            if (rtsBase.hasAuthority)
            {
                return;
            } 
            
            Debug.Log("damage to "+col.name);
            col.GetComponent<RTSBase>().DealDamage(damage);

        }

   
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.gameObject.transform.position,effectRadious );    }
    }
 

