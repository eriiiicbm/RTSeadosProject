
using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
   [SerializeField] private Health health = null;
   [SerializeField] private int resourceCost = 10;
   [SerializeField] private UnitMovement unitMovement;
   [SerializeField] private UnityEvent onSelected;
   [SerializeField] private UnityEvent onDeselected;
   [SerializeField] private Targeter targeter;
   [SerializeField] public  static  event Action<Unit> ServerOnUnitSpawned;
   [SerializeField] public  static  event Action<Unit> ServerOnUnitDespawned;
  [SerializeField] public  static  event Action<Unit> AuthorityOnUnitSpawned;
   [SerializeField] public  static  event Action<Unit> AuthorityOnUnitDespawned;

   public int GetResourceCost()
   {
      return resourceCost;
   }

   public UnitMovement GetUnitMovement()
   {
      return unitMovement;
   }  public Targeter GetTargeter()
   {
      return targeter;
   }

   #region Server

   public override void OnStartServer()
   {
     ServerOnUnitSpawned?.Invoke(this);
     health.ServerOnDie += ServerHandleDie;

   }

   public override void OnStopServer()
   {

      ServerOnUnitDespawned?.Invoke(this);
      health.ServerOnDie -= ServerHandleDie;

   }
   [Server]
   private void ServerHandleDie()
   {
      NetworkServer.Destroy(gameObject);    }
   #endregion
   #region Client

   public override void OnStartAuthority()
   {
      AuthorityOnUnitSpawned?.Invoke(this);
   }

   public override void OnStopClient()
   {
      if (  !hasAuthority)
      {return;
      }
      AuthorityOnUnitDespawned?.Invoke(this);
   }

   [Client]
   public void Select()
   {
      if (!hasAuthority)
      {
         return;
      }
      onSelected?.Invoke();
   }

   [Client]
   public void Deselect()
   {
      if (!hasAuthority)
      {
         return;
      }
      onDeselected?.Invoke();

   }
   
   #endregion
}
