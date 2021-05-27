using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PassiveAbility : MonoBehaviour
{
    public float efectRadius;
    public float recoverySpeed;
    public List<Unit> units;

    #region Server

    

    #endregion
    // Start is called before the first frame update
 
    private void Start()
    {
        efectRadius = GetComponent<RTSBase>().rtsEntity.EffectRadious;
        recoverySpeed = GetComponent<RTSBase>().rtsEntity.RecoverySpeed;
    }

    // Update is called once per frame
   [ServerCallback]
    void Update()
    {
        RaycastHit[] hits = Physics.SphereCastAll(gameObject.transform.position, efectRadius, gameObject.transform.forward);

        units = new List<Unit>();

        foreach (RaycastHit hit in hits){
            RaycastToUnit(hit);
        }

        if (units.Count >= 0)
        {
            foreach (var unit in units)
            { 
                if(unit != null)PasiveEffect(unit);

                StartCoroutine(Wait(0.5f));
            }
        }
        units.Clear();
    }

    private void RaycastToUnit(RaycastHit hit)
    {
        Unit unit = hit.collider.GetComponent<Unit>();

        if (unit == null) return;

        if (units.Contains(unit)) return;
        if (unit.gameObject==this.gameObject)
        {
            return;
        }
        Debug.Log(name + " ha detectado a " + hit.collider.name);

        units.Add(unit);
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

    public virtual void PasiveEffect(Unit unit)
    {
        Debug.LogError("OverrideThisMethod before use it");
    }
}
