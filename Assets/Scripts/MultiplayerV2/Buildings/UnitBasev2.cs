using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBasev2 : Building
{
    [SerializeField] public static event Action<int> ServerOnPlayerDie;
    [SerializeField] public static event Action<UnitBasev2> ServerOnBaseSpawned;
    [SerializeField] public static event Action<UnitBasev2> ServerOnBaseDespawned;


    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnRTSDie += ServerHandleDie;
        RTSPlayerv2.ClientOnResourcesUpdated += HandleResourcesUpdated;
        ServerOnBaseSpawned?.Invoke(this);

    }

    private void HandleResourcesUpdated(SyncList<int> obj)
    {
        RTSPlayerv2 playerv2 = connectionToClient.identity.GetComponent<RTSPlayerv2>();
        DealDamage(float.MaxValue);
        
    }

    public override void OnStopServer()
    {

        base.OnStopServer();
        ServerOnRTSDie -= ServerHandleDie;
        RTSPlayerv2.ClientOnResourcesUpdated -= HandleResourcesUpdated;

    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);    }

    #endregion
 

    #region Client

 

 #endregion
}
