using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BuildingN : NetworkBehaviour
{
 [SerializeField] private GameObject buildingPreview;
 [SerializeField] private Sprite icon;
 [SerializeField] private int price=100;
 [SerializeField] private int id=-1;
 [SerializeField] public  static  event Action<BuildingN> ServerOnBuildingSpawned;
 [SerializeField] public  static  event Action<BuildingN> ServerOnBuildingDespawned;
 [SerializeField] public  static  event Action<BuildingN> AuthorityOnBuildingSpawned;
 [SerializeField] public  static  event Action<BuildingN> AuthorityOnBuildingDespawned;

 public GameObject getBuildingPreview()
 {
  return buildingPreview;
 }public Sprite GetIcon()
 {
  return icon;
 }
 public int GetId()
 {
  return id;
 } public int GetPrice()
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
