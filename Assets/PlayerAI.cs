using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : MonoBehaviour
{
    private RTSPlayerv2 rtsPlayerv2;
    // Start is called before the first frame update
    void Start()
    {
        rtsPlayerv2 = GetComponent<RTSPlayerv2>();
    /*    rtsPlayerv2.OnStartServer();
        rtsPlayerv2.OnStartAuthority();
        rtsPlayerv2.OnStartClient();
    */}

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
     //   rtsPlayerv2.OnStopClient();
     //   rtsPlayerv2.OnStopServer();
    }
}
