using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
 [SerializeField] private Health health = null;
 [SerializeField] public  static  event Action<UnitBase> ServerOnBaseSpawned;
 [SerializeField] public  static  event Action<UnitBase> ServerOnBaseDespawned;
 

 #region Server
 public override void OnStartServer()
 {
   health.ServerOnDie += ServerHandleDie;
   ServerOnBaseSpawned?.Invoke(this);

 }

 public override void OnStopServer()
 {

     ServerOnBaseDespawned?.Invoke(this);
   health.ServerOnDie -= ServerHandleDie;

 }
 [Server]
 private void ServerHandleDie()
 {
     NetworkServer.Destroy(gameObject);    }
 // Start is called before the first frame update
 void Start()
 {
        
 }

 // Update is called once per frame
 void Update()
 {
        
 }

 #endregion

 #region Client

 

 #endregion
}
