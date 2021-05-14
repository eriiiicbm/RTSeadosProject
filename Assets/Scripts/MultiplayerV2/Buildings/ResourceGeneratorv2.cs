using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ResourceGeneratorv2 : Building
{
 
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;

    private RTSPlayerv2 player;

    public override void OnStartServer()
    {
        base.OnStartServer();
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
        ServerOnRTSDie += ServerHandleDie;
        GameOverHandlerv2.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        ServerOnRTSDie -= ServerHandleDie;
        GameOverHandlerv2.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer<=0)
        {
            player.SetResources(player.GetResources(ResourcesType.Wood) + resourcesPerInterval, ResourcesType.Wood);

            timer += interval;
        }
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    
}
