using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AreaDamage : NetworkBehaviour, ComponentAbility
{
    [Server]
    public void active(RTSBase target, float damage)
    {
        Debug.Log("damageArea con damage: " + damage);

        RaycastHit [] hits =  Physics.SphereCastAll(target.gameObject.transform.position, GetComponent<RTSBase>().rtsEntity.EffectRadious, target.gameObject.transform.forward);

        if (hits.Length > 0) return;

        foreach (RaycastHit hit in hits)
        {
            Debug.Log("try of damage to " + hit.collider.name);

            if (hit.collider != null) return;
            if (hit.collider.GetComponent<RTSBase>() != null) return;
            if (hit.collider.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient)return;                
            }

            Debug.Log("damage to "+hit.collider.name);

            hit.collider.GetComponent<RTSBase>().DealDamage(damage);
        }
        
    }
}
