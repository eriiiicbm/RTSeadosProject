using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBasev2 : Building
{
  [SerializeField] public  static  event Action<int> ServerOnPlayerDie;
 [SerializeField] public  static  event Action<UnitBasev2> ServerOnBaseSpawned;
 [SerializeField] public  static  event Action<UnitBasev2> ServerOnBaseDespawned;
 

 #region Server
 public override void OnStartServer()
 {
     base.OnStartServer();
   ServerOnRTSDie += ServerHandleDie;
   ServerOnBaseSpawned?.Invoke(this);
       
 }

 public override void OnStopServer()
 {

     base.OnStopServer();
   ServerOnRTSDie -= ServerHandleDie;

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
