using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private BuildingN[] buildings = new BuildingN[0];
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private float buildingRangeLimit = 5;
    [SerializeField] private List<UnitN> myUnits = new List<UnitN>();
    [SerializeField] private List<BuildingN> myBuildings = new List<BuildingN>();
    private Color teamColor = new Color();
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;

    public event Action<int> ClientOnResourcesUpdated;

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public int GetResources()
    {
        return resources;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public List<UnitN> GetMyUnits()
    {
        return myUnits;
    }

    public List<BuildingN> GetMyBuildings()
    {
        return myBuildings;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity,
            buildingBlockLayer))
        {
            return false;
        }


         foreach (var b in myBuildings)
        {
            if ((point - buildingCollider.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    #region Server

    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor=newTeamColor;
    }
    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    public override void OnStartServer()
    {
        UnitN.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        UnitN.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        BuildingN.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        BuildingN.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }


    public override void OnStopServer()
    {
        UnitN.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        UnitN.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        BuildingN.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        BuildingN.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        BuildingN buildingToPlace = null;
        foreach (var building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if (buildingToPlace == null)
        {
            return;
        }

        if (resources < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();


        if (!CanPlaceBuilding(buildingCollider,point))
        {
            return;
        }

        SetResources(resources - buildingToPlace.GetPrice());
        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    private void ServerHandleUnitSpawned(UnitN unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(UnitN unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingDespawned(BuildingN building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myBuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawned(BuildingN building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
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

        UnitN.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        UnitN.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        BuildingN.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        BuildingN.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        UnitN.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        UnitN.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        BuildingN.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        BuildingN.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void AuthorityHandleBuildingDespawned(BuildingN obj)
    {
        myBuildings.Remove(obj);
    }

    private void AuthorityHandleBuildingSpawned(BuildingN obj)
    {
        myBuildings.Add(obj);
    }

    private void AuthorityHandleUnitSpawned(UnitN unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(UnitN unit)
    {
        myUnits.Remove(unit);
    }

    #endregion
}