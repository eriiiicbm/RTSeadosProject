
using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
   [SerializeField] private UnitMovement unitMovement;
   [SerializeField] private UnityEvent onSelected;
   [SerializeField] private UnityEvent onDeselected;
   [SerializeField] public  static  event Action<Unit> ServerOnUnitSpawned;
   [SerializeField] public  static  event Action<Unit> ServerOnUnitDespawned;
  [SerializeField] public  static  event Action<Unit> AuthorityOnUnitSpawned;
   [SerializeField] public  static  event Action<Unit> AuthorityOnUnitDespawned;

   public UnitMovement GetUnitMovement()
   {
      return unitMovement;
   }

   #region Server

   public override void OnStartServer()
   {
     ServerOnUnitSpawned?.Invoke(this);
   }

   public override void OnStopServer()
   {

      ServerOnUnitDespawned?.Invoke(this);
      
   }

   #endregion
   #region Client

   public override void OnStartClient()
   {
      if (!isClientOnly || !hasAuthority)
      {return;
      }
      AuthorityOnUnitSpawned?.Invoke(this);
   }

   public override void OnStopClient()
   {
      if (!isClientOnly || !hasAuthority)
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
