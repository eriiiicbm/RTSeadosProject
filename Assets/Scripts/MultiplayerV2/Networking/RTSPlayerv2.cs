using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class RTSPlayerv2 : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private Unit[] units = new Unit[0];
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private float buildingRangeLimit = 5;
    [SerializeField] private List<Unit> myUnits = new List<Unit>();
    [SerializeField] private List<Building> myBuildings = new List<Building>();
    private Color teamColor = new Color();

    public SyncList<int> resources = new SyncList<int>(){9999,9999,9999,9999};

    [SyncVar]
    [SerializeField] private int trops = 0;
    
    [SyncVar]
    [SerializeField] private int maxTrops = 6;
  
    [SyncVar]
    [SerializeField] private int tropsInProduction = 6;
 
    [SyncVar]
    [SerializeField] private int numHouse = 0;

    [SyncVar]
    [SerializeField] private int maxNumHouse = 20;
    [SerializeField] private bool hero1;
  [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
[SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;
    public static event Action ClientOnInfoUpdated;
    public static event Action<SyncList<int>> ClientOnResourcesUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public Unit FindUnitById(int id)
    {
        return units.FirstOrDefault(unit => unit.GetId() == id);
    }
    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public SyncList<int> GetAllResources()
    {
        return resources;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

 

    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public int GetResources(ResourcesType resourceType)
    {
        switch (resourceType)
        {
            case ResourcesType.Ingredients:
                return resources[0];

            case ResourcesType.SubstanceX:
                return resources[1];

            case ResourcesType.Wood:
                return resources[2];

            case ResourcesType.Stone:
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
    public void SetResources(int newResources, ResourcesType resourceType)
    {       
Debug.Log("Set resources");
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
        ClientOnResourcesUpdated?.Invoke(resources);

    }
    [Server]
    public void SetDisplayName(string newName)
    {
        displayName = newName;
    }
    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
        DontDestroyOnLoad(gameObject);
        ClientOnResourcesUpdated?.Invoke(resources);

    }


    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;


    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner)
        {
            return;
        }

        ((RTSNetworkManagerv2)NetworkManager.singleton).StartGame();
    }

    [Command]
    public void CmdTryCreateUnit(int unitId, UnitSpawnerv3 spawner)
    {
        if (!CheckIfUserHasSpaceTrop()) return;
        Unit unit = FindUnitById(unitId);

        if (!CheckIfUserHasResources(unit.prices))
        {
            return;
        }

        if (spawner.netIdentity==this.netIdentity)
        {
            Debug.Log("Is yours");
        }
        Debug.Log("line 192");
        spawner.AddUnitToTheQueue(unitId);
      
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
Debug.Log("here before boom 1");
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
        Debug.Log("here before boom 2");
        if(buildingToPlace.GetComponent<Fridge>() != null)
        {
            if (!connectionToClient.identity.GetComponent<RTSPlayerv2>().AddHouse()) return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();


        if (!CanPlaceBuilding(buildingCollider, point))
        {
            Debug.Log("Cant build here");
            return;
        }

        RestPriceToResources(buildingToPlace.GetPrice());
 
        GameObject buildingInstance =
            Instantiate(buildingToPlace.gameObject, point + buildingToPlace.transform.position,
                buildingToPlace.transform.rotation);
        Debug.LogWarning("Build success in " + point);
 
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    public bool CheckIfUserHasResources(List<int> prices)
    {
        if (prices.Count < resources.Count)
        {
            Debug.Log("Price " + prices.Count + "  Resources num " + resources.Count);
            return false;
        }

        for (var i = 0; i < resources.Count; i++)
        {
            if (resources[i] - prices[i] >= 0) continue;
            Debug.Log("You are poor");
            return false;
        }

        return true;
    }

    public void RestPriceToResources(List<int> prices)
    {
        Debug.Log("apply price + " +String.Join("\n", prices)  + " resources" + resources);
        for (int i = 0; i < resources.Count; i++)
        {
            resources[i] -= prices[i];
        }
        Debug.Log("applied price + " +String.Join("\n", prices) + " resources" + resources);

        Debug.Log("resource updated");
        ClientOnResourcesUpdated?.Invoke(resources);

    }

    public bool CheckIfUserHasSpaceTrop()
    {
        if ((trops + tropsInProduction )< maxTrops)
        {
            tropsInProduction++;
            return true;
        }
 
        Debug.Log("Your food can't rest");
        return false;
    }

   [Server]
    void AddTrops()
    {
        
        trops++;
        if (tropsInProduction!=0)
            tropsInProduction--;
        Debug.Log("resources 0" + resources[0]);
        ClientOnResourcesUpdated?.Invoke(resources);

    }

    public bool CheckIfUserHasSpaceHouse()
    {
        if (numHouse < maxNumHouse) return true;

        Debug.Log("Much house");
        return false;
    }

    public bool AddHouse()
    {
        if (!CheckIfUserHasSpaceHouse()) return false;

        numHouse++;
        ClientOnResourcesUpdated?.Invoke(resources);

        return true;
    }

    public void DeleteHouse()
    {
        numHouse--;
        maxTrops -= 3;
        ClientOnResourcesUpdated?.Invoke(resources);

    }

    public int Trops
    {
        get => trops;
        set => trops = value;
    }

    public int MaxTrops
    {
        get => maxTrops;
        set => maxTrops = value;
    }

    public bool Hero1
    {
        get => hero1;
        set => hero1 = value;
    }

    public int MaxNumHouse
    {
        get => maxNumHouse;
        set => maxNumHouse = value;
    }

    public int NumHouse
    {
        get => numHouse;
        set => numHouse = value;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        myUnits.Add(unit);
        AddTrops();

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
    public override void OnStartClient()
    {
        if (NetworkServer.active)
        {
            return;
        }
        DontDestroyOnLoad(gameObject);
        ((RTSNetworkManagerv2) NetworkManager.singleton).Players.Add(this);
    }
    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly )
        {
            return;
        }
        ((RTSNetworkManagerv2) NetworkManager.singleton).Players.Remove(this);
        if (!hasAuthority)
        {
            return;
        }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }



    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority)
        {
            return;
        }        
        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void AuthorityHandleBuildingDespawned(Building obj)
    {
        myBuildings.Remove(obj);
    }

    [Client]
    private void AuthorityHandleBuildingSpawned(Building obj)
    {
        myBuildings.Add(obj);
    }

    [Client]
    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
        ClientOnResourcesUpdated?.Invoke(resources);
Debug.Log("This is");
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void ClientHandleDisplayNameUpdated(string oldDisplayName, string newDisplayName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    #endregion
}