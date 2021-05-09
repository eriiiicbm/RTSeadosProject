using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings = new Building[0];
 [SerializeField]   private List<Unit> myUnits = new List<Unit>();
 [SerializeField]   private List<Building> myBuildings = new List<Building>();
 [SyncVar(hook    =nameof(ClientHandleResourcesUpdated))] private int resources=500;
 public event  Action<int> ClientOnResourcesUpdated;
 public int GetResources()
 {
     return resources;
 }public List<Unit> GetMyUnits()
 {
     return myUnits;
 }
 public List<Building> GetMyBuildings()
 {
     return myBuildings;
 }


 #region Server
[Server]
public void SetResources(int newResources)
 {
     resources = newResources;
 }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }


    public override void OnStopServer()
    {
      
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;

    }

   [Command]
   public void CmdTryPlaceBuilding(int buildingId,Vector3 point)
   {
       Building buildingToPlace = null;
       foreach (var building in buildings)
       {
           if (building.getId() == buildingId)
           {
               buildingToPlace = building;
               break;
           }
       }

       if (buildingToPlace==null)
           {
           return;    
           }

        GameObject buildingInstance  = Instantiate(buildingToPlace.gameObject  ,point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance,connectionToClient);
       }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId!=connectionToClient.connectionId)
        {
            return;
        }
        myUnits.Add(unit);
    }  private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId!=connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Remove(unit);

    }
    
    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId!=connectionToClient.connectionId)
        {
            return;
        }
        myBuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId!=connectionToClient.connectionId)
        {
            return;
        }
        myBuildings.Add(building);
    }
    
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        if (NetworkServer.active)
        {
      return;
      
        }
        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;  
        
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly||!hasAuthority)
        {
            return;
      
        }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;

    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }
    private void AuthorityHandleBuildingDespawned(Building obj)
    {
        myBuildings.Remove(obj);
    }

    private void AuthorityHandleBuildingSpawned(Building obj)
    {
        myBuildings.Add(obj);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
 
        myUnits.Add(unit);
    } 
    private void AuthorityHandleUnitDespawned(Unit unit)
    {
      
        myUnits.Remove(unit);

    }
    #endregion
}
