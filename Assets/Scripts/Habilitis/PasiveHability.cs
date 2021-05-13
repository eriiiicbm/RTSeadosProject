using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasiveHability : MonoBehaviour
{
    public float efectRadius;
    bool inTrigger;
    public float recoverySpeed;
    public List<Unit> units = new List<Unit>();

    // Start is called before the first frame update
    private void Start()
    {
        efectRadius = GetComponent<RTSEntity>().EffectRadious;
        recoverySpeed = GetComponent<RTSEntity>().RecoverySpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
