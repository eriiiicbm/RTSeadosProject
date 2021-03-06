using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
 
public class AreaDamage : NetworkBehaviour, ComponentAbility
{   
    [SerializeField] private LayerMask layerMask =~8;

    private float effectRadious=20f;
    public void SetEffectRadious(float newEffectRadious)
    {
      effectRadious= newEffectRadious;
    }

    public void SetParameter(float parameter)
    {
        effectRadious = parameter;
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
            if (rtsBase==null)
            {
               return; 
            }

            if (rtsBase.connectionToClient==null)
            {
            goto    damage;
            }
            if (rtsBase.connectionToClient.connectionId==connectionToClient.connectionId)
            {
                return;
            } 
            damage:
            Debug.Log("damage to "+col.name);
            col.GetComponent<RTSBase>().DealDamage(damage);

        }

   
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.gameObject.transform.position,effectRadious );    }
    }
 

