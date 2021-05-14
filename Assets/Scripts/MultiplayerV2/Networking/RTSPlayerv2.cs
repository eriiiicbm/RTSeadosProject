using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayerv2 : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private float buildingRangeLimit = 5;
    [SerializeField] private List<Unit> myUnits = new List<Unit>();
    [SerializeField] private List<Building> myBuildings = new List<Building>();
    private Color teamColor = new Color();

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    [SerializeField] private List<int> resources = new List<int>();
    [SerializeField] private int trops = 6;
    [SerializeField] private int maxTrops = 10;
    [SerializeField] private int numHouse = 0;
    [SerializeField] private int maxNumHouse = 20;
    [SerializeField] private bool hero1;

    public event Action<List<int>> ClientOnResourcesUpdated;

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }
    public List<int> GetAllResources() {
        return resources;
    }
    public int GetResources(ResourcesType resourceType)
    {
        switch (resourceType) {
            case ResourcesType.Ingredients:
                return resources[0];
           
            case ResourcesType.Stone:
                return resources[1];
                 
            case ResourcesType.SubstanceX:
                return resources[2];
                 
            case ResourcesType.Wood:
                return resources[3];
                 
        }
        return -1;
          
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public List<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public List<Building> GetMyBuildings()
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
        teamColor = newTeamColor;
    }

    [Server]
    public void SetResources(int newResources,ResourcesType resourceType)
    {
        switch (resourceType)
        {
            case ResourcesType.Ingredients:
               resources[0] = newResources;
                break;
            case ResourcesType.SubstanceX:
                 resources[1] = newResources;
                break;
            case ResourcesType.Wood:
                  resources[2] = newResources;
                break;
            case ResourcesType.Stone:
                resources[3] = newResources;
                break;
        }
      
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
    public void CmdTryPlaceBuilding(int buildingId, Vector3 point)
    {
        Building buildingToPlace = null;
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
            Debug.LogWarning("Building to place is null");
            return;
        }
        
        if (!CheckIfUserHasResources(buildingToPlace.GetPrice()))
        {
            Debug.LogWarning("You are poor");

            return;
        }

        if(connectionToClient.identity.GetComponent<RTSPlayerv2>().addHouse()) return;

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();


        if (!CanPlaceBuilding(buildingCollider, point))
        {
            Debug.Log("Cant build here");
            return;
        }

        RestPriceToResources(buildingToPlace.GetPrice());
        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        Debug.LogWarning("Build success in " + point);

        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    public bool CheckIfUserHasResources(List<int> prices)
    {
         if (prices.Count < resources.Count) {
            Debug.Log("Price " + prices.Count + "  Resources num " + resources.Count);
            return false;
        }
        for (int i=0; i < resources.Count; i++) {
            if (resources[i] - prices[i] < 0) {
                Debug.Log("You are poor");
                return false;

            }
        }
        return true;
    }

    public void RestPriceToResources(List<int> prices) {
        for (int i = 0; i < resources.Count; i++)
        {
            resources[i] -= prices[i];
        }
    }

    public bool checkIfUserHasSpaceTrop()
    {
        if (trops < maxTrops) return true;

        Debug.Log("Your food can't rest");
        return false;
    }

    public bool checkIfUserHasSpaceHouse()
    {
        if (numHouse < maxNumHouse) return true;

        Debug.Log("Much house");
        return false;
    }

    public bool addHouse()
    {
        if (checkIfUserHasSpaceHouse()) return false;

        numHouse++;
        return true;
    }

    public void deleteHouse()
    {
        numHouse--;
        maxTrops -= 3;
    }

    public int Trops { get => trops; set => trops = value; }
    public int MaxTrops { get => maxTrops; set => maxTrops = value; }
    public bool Hero1 { get => hero1; set => hero1 = value; }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myBuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
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

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void ClientHandleResourcesUpdated(List<int> oldResources, List<int> newResources)
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