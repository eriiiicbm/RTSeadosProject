using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Building : NetworkBehaviour
{
 [SerializeField] private GameObject buildingPreview;
 [SerializeField] private Sprite icon;
 [SerializeField] private int price=100;
 [SerializeField] private int id=-1;
 [SerializeField] public  static  event Action<Building> ServerOnBuildingSpawned;
 [SerializeField] public  static  event Action<Building> ServerOnBuildingDespawned;
 [SerializeField] public  static  event Action<Building> AuthorityOnBuildingSpawned;
 [SerializeField] public  static  event Action<Building> AuthorityOnBuildingDespawned;

 public GameObject getBuildingPreview()
 {
  return buildingPreview;
 }public Sprite getIcon()
 {
  return icon;
 }
 public int getId()
 {
  return id;
 } public int getPrice()
 {
  return price;
 }

 #region Server

 public override void OnStartServer()
 {
ServerOnBuildingSpawned?.Invoke(this); }

 public override void OnStopServer()
 {
  ServerOnBuildingDespawned?.Invoke(this); }

 #endregion

 #region Client

 public override void OnStartAuthority()
 {
  AuthorityOnBuildingSpawned?.Invoke(this);
 }

 public override void OnStopClient()
 {
  if (  !hasAuthority)
  {return;
  }
  AuthorityOnBuildingDespawned?.Invoke(this);
 }


 #endregion
}
