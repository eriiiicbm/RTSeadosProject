using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour, ComponetHability
{
    public void active(RTSBase target, float damage)
    {
         RaycastHit [] hits =  Physics.SphereCastAll(target.gameObject.transform.position, GetComponent<RTSBase>().rtsEntity.EffectRadious, target.gameObject.transform.forward);

        if (hits.Length > 0) return;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null) return;
            if (hit.collider.GetComponent<RTSBase>() != null) return;

            hit.collider.GetComponent<RTSBase>().DealDamage(damage);
        }
        
    }
}
