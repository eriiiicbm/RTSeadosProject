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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit rtsaBase = other.GetComponent<Unit>();
        units.Add(rtsaBase);
    }
    private void OnTriggerExit(Collider other)
    {
        Unit rtsaBase = other.GetComponent<Unit>();
        units.Remove(rtsaBase);

    }

    public IEnumerator Wait(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
}
