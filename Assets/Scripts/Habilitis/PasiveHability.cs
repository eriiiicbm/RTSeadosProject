using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasiveHability : MonoBehaviour
{
    public float efectRadius;
    public float recoverySpeed;
    public List<Unit> units = new List<Unit>();

    // Start is called before the first frame update
    private void Start()
    {
        efectRadius = GetComponent<RTSBase>().rtsEntity.EffectRadious;
        recoverySpeed = GetComponent<RTSBase>().rtsEntity.RecoverySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, efectRadius, gameObject.transform.forward);

        foreach (RaycastHit hit in hits){
            units.Add(RaycastToUnit(hit));
        }

        PasiveEffect();
    }

    private Unit RaycastToUnit(RaycastHit hit)
    {
        if (hit.collider.GetComponent<Unit>() == null) return null;
        Debug.Log(name + " ha detectado a " + hit.collider.name);

        return hit.collider.GetComponent<Unit>();
    }

    /*private void OnTriggerEnter(Collider other)
    {
        Debug.Log(name + " ha detectado a "+ other.name);

        Unit rtsaBase = other.GetComponent<Unit>();
        units.Add(rtsaBase);
    }

    private void OnTriggerExit(Collider other)
    {
        Unit rtsaBase = other.GetComponent<Unit>();
        units.Remove(rtsaBase);
    }*/

    public IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    public virtual void PasiveEffect()
    {
        Debug.LogError("OverrideThisMethod before use it");
    }
}
