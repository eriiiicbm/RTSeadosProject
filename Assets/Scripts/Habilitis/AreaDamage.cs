using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class AreaDamage : NetworkBehaviour, ComponetHability
{
    [Server]
    public void active(RTSBase target, float damage)
    {
         RaycastHit [] hits =  Physics.SphereCastAll(target.gameObject.transform.position, GetComponent<RTSBase>().rtsEntity.EffectRadious, target.gameObject.transform.forward);

        if (hits.Length > 0) return;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null) return;
            if (hit.collider.GetComponent<RTSBase>() != null) return;
            if (hit.collider.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
            {
                if (networkIdentity.connectionToClient == connectionToClient)return;                
            }

                hit.collider.GetComponent<RTSBase>().DealDamage(damage);
        }
        
    }
}
