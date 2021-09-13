using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssetUsageDetectorNamespace;
using Cinemachine;
using Mirror;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

public class PlayerAI : NetworkBehaviour
{
    private CinemachineVirtualCamera camera;

    private RTSPlayerv2 rtsPlayerv2;

    // Start is called before the first frame update
    [SyncVar] public AIStates currentState = AIStates.Nothing;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private UnitCommandGiverv2 unitCommandGiverv2;
    private UnitSelectionHandlerv2 unitSelectionHandlerv2;

    void Start()
    {
    }

    public void SetARandomState()
    {
        while (true)
        {
            var lastState = currentState;

            currentState = (AIStates) UnityEngine.Random.Range(0, (int) AIStates.CommandingUnits);
            if (currentState == lastState)
            {
                continue;
            }

            break;
        }
    }


    public IEnumerator StartAI()
    {
        unitCommandGiverv2 = FindObjectOfType<UnitCommandGiverv2>();
        unitSelectionHandlerv2 = FindObjectOfType<UnitSelectionHandlerv2>();

        // currentState = (AIStates) UnityEngine.Random.Range(0, (int) AIStates.PickingResources);
        yield return new WaitForSeconds(0.9f);
        // currentState = (AIStates)  AIStates.PlacingBuildings;
        SetARandomState();
        Debug.Log($"AI STARTES {currentState}");
        yield return new WaitForEndOfFrame();
        GoToNextState();
    }

    public bool isTimeOut = false;
    public float timeOut = 5f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(currentRaY);
    }

    private Ray currentRaY = new Ray();

    [Client]
    public IEnumerator PlaceBuildings()
    {
        Debug.Log("IN PLACE BUILDINGS");

        bool placed = false;
        StartCoroutine(nameof(RunTimeOut));
        do
        {
            // GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToServer);
            Building b = GetARandomBuilding();
            Villager v = GetARandomVillager();
            Vector3 pos = GetAPosNearVillager(v, 100);
            Debug.Log($"will spawn arround pos {pos}");
            Ray ray = new Ray(pos, Vector3.down);
            currentRaY = ray;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                Debug.LogWarning($"Will place build with id {b.GetId()}");
                if (!rtsPlayerv2.CanPlaceBuilding(b.GetComponent<BoxCollider>(), hit.point)
                )
                {
                    Debug.Log("cant place here");
                    placed = false;
                    continue;
                    yield return 0;
                }

                rtsPlayerv2.CmdTryPlaceBuilding(b.GetId(), hit.point);
                placed = rtsPlayerv2.lastOperationWorked;
            }

            yield return new WaitForEndOfFrame();
            //  GetComponent<NetworkIdentity>().RemoveClientAuthority();
        } while (!placed || !isTimeOut);

        Debug.Log("AI PLACED A BUILD or not");

        StopCoroutine(nameof(RunTimeOut));
        isTimeOut = false;
        currentState = AIStates.Nothing;
    }

    public IEnumerator RunTimeOut()
    {
        yield return new WaitForSeconds(timeOut);
        isTimeOut = true;
        Debug.Log("Time OUT");
        yield break;
    }

    public void LookAtUnit(Unit unit)
    {
        this.transform.position = unit.transform.position
            ;
    }

    public Vector3 GetAPosNearVillager(Villager villager, int radious)
    {
        Vector3 insideUnit = UnityEngine.Random.insideUnitSphere;
        Debug.Log(
            $"{villager.transform.position} villager pos  and a radious of {radious} , insideUnity {insideUnit}   final{villager.transform.position + insideUnit * radious}  ");
        return
            villager.transform.position + insideUnit * radious;
    }

    Building GetARandomSpawnedBuilding()
    {
        List<Building> myBuildings = rtsPlayerv2.GetMyBuildings();
        if (myBuildings == null)
        {
            return null;
        }

        return myBuildings[UnityEngine.Random.Range(0, myBuildings.Count)];
    }

    Building GetARandomBuilding()
    {
        List<Building> buildings = rtsPlayerv2.GetBuildings();
        if (buildings == null)
        {
            return null;
        }

        return buildings[UnityEngine.Random.Range(0, buildings.Count)];
    }

    RTSBase GetARandomUnit()
    {
        List<Unit> myUnits = rtsPlayerv2.GetUnits();

        return myUnits[UnityEngine.Random.Range(0, myUnits.Count)];
    }

    private RTSBase GetARandomMyUnit()
    {
        List<Unit> myUnits = rtsPlayerv2.GetMyUnits();

        return myUnits[UnityEngine.Random.Range(0, myUnits.Count)];
    }

    private List<Villager> GetVillagersList()
    {
        List<Unit> myUnits = rtsPlayerv2.GetMyUnits();
        List<Villager> villagers = new List<Villager>();
        foreach (var unit in myUnits)
        {
            Villager villager = unit.GetComponent<Villager>();
            if (villager != null)
            {
                villagers.Add(villager);
            }
        }


        return villagers;
    }

    private Villager GetARandomVillager()
    {
        List<Villager> villagers = GetVillagersList();
        foreach (var VARIABLE in villagers)
        {
            Debug.Log(VARIABLE.ToString());
        }

        return villagers[UnityEngine.Random.Range(0, villagers.Count)];
    }

    private Villager GetARandomNiniVillager()
    {
        while (true)
        {
            var villager = GetARandomVillager();
            if (villager.unitStates == UnitStates.Attack) continue;
            return villager;
            break;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        //   rtsPlayerv2.OnStopClient();
        //   rtsPlayerv2.OnStopServer();
    }

    IEnumerator NothingState()
    {
        Debug.Log("Nothing: Enter");

        while (this.currentState == AIStates.Nothing)
        {
            //   this.currentState = GetDistance() < followRange ? UnitStates.Follow : UnitStates.Idle;

            yield return new WaitForSeconds(1);
            SetARandomState();
        }

        Debug.Log("Nothing: Exit");
        GoToNextState();
    }

    IEnumerator PickingResourcesState()
    {
        Debug.Log("Pick Resources Enter");

        while (this.currentState == AIStates.PickingResources)
        {
            //   this.currentState = GetDistance() < followRange ? UnitStates.Follow : UnitStates.Idle;

            yield return new WaitForSeconds(1);
            SetARandomState();
        }

        Debug.Log("PICKRESOURRCES: Exit");
        GoToNextState();
    }

    IEnumerator PlacingBuildingsState()
    {
        //todo añadir lista de cosas pendientes a construir
        Debug.Log("Place build  Enter");

        while (this.currentState == AIStates.PlacingBuildings)
        {
            Debug.Log("Place build  Continue");

            yield return
                StartCoroutine(nameof(PlaceBuildings));
            yield return new WaitForSeconds(0.3f);
        }

        Debug.Log("Place build: Exit");
        GoToNextState();
    }

    public List<Building> GetNonBuildedBuildings()
    {
        var buildings = rtsPlayerv2.GetMyBuildings();

        return buildings.Where(building => !building.builded).ToList();
    }

    IEnumerator BuildingBuildingsState()
    {
     

        while (this.currentState == AIStates.BuildingBuildings)
        {
            List<Building> buildings = GetNonBuildedBuildings();
            if (buildings == null|| buildings.IsEmpty()  )
            {
                SetARandomState();
                Debug.Log("The isnt buildings to build");
            }

            //todo añadir lista de cosas pendientes a construir
            Debug.Log("Place build  Enter");
            Debug.Log("Place build  Continue");
            Villager villager = GetARandomNiniVillager(); 
            Debug.Log($"Unbuilded build 0 {buildings[0].name}]");

            Debug.Log($"´{villager.name} goes to position {buildings[0].transform.position} where there is a {buildings[0].name}");
            villager.GetTargeter().CmdSetTarget(buildings[0].gameObject);
            // villager.CmdMove(buildings[0].transform.position);
           
             Debug.Log("Sended cmmove");
 
            yield return new WaitForEndOfFrame();
            villager.building = buildings[0];
        }

        Debug.Log("Place build: Exit");
        GoToNextState();
    }

    IEnumerator WaitState()
    {
        List<Building> buildings = GetNonBuildedBuildings();

        Debug.Log("Wait  Enter");

        while (this.currentState == AIStates.Wait)
        {
            Debug.Log("Wait  Continue");

            yield return new WaitForSeconds(3f);
            SetARandomState();
        }

        Debug.Log("Wait Exit");
        GoToNextState();
    }

    void GoToNextState()
    {
        string methodName = this.currentState.ToString() + "State";
        Debug.Log("AI STATE " + methodName);
        isTimeOut = false;
        SendMessage(methodName);
    }

    public enum AIStates : int
    {
        Nothing = 0,
        Wait = 1,
        PickingResources = 2,
        CommandingUnits = 5,
        SelectingUnits = 6,
        PlacingBuildings = 3,
        BuildingBuildings = 4
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        rtsPlayerv2 = GetComponent<RTSPlayerv2>();
        camera = rtsPlayerv2.GetCameraTransform().GetComponent<CinemachineVirtualCamera>();
        /*    rtsPlayerv2.OnStartServer();
            rtsPlayerv2.OnStartAuthority();
            rtsPlayerv2.OnStartClient();
        */
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(StartAI));
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    private int unitGameplayId = 0;
    private int buildingGameplayId = 0;
    private void ServerHandleUnitSpawned(Unit unit)
    {
        if (unit.connectionToClient?.connectionId != connectionToClient.connectionId)
        {
            return;
        }

        unit.name += unitGameplayId  + "   "+  connectionToClient.connectionId;
        unitGameplayId++;
        rtsPlayerv2.GetMyUnits().Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        building.name += buildingGameplayId+ "   "+  connectionToClient.connectionId;
        buildingGameplayId++;

    }

    public void DetectEnemy(float attackRange)
    {
        Collider[] collider = Physics.OverlapSphere(this.gameObject.transform.position, attackRange, layerMask);
        if (collider.Length == 0)
        {
            return;
        }

        Collider nearCollider = collider[0];
        foreach (var col in collider)
        {
            if (col.GetComponent<RTSBase>().connectionToClient == null)
            {
                continue;
            }

            if (col.GetComponent<RTSBase>().connectionToClient.connectionId ==
                NetworkClient.connection.connectionId) continue;
            if (Vector3.Distance(col.transform.position, this.transform.position) <=
                Vector3.Distance(nearCollider.transform.position, this.transform.position))
            {
                nearCollider = col;
            }
        }

        if (nearCollider == null) return;

        RTSBase enemy = nearCollider.GetComponent<RTSBase>();

        if (enemy == null) return;
        if (enemy.connectionToClient == null)
        {
            return;
        }

        if (enemy.connectionToClient.connectionId ==
            connectionToClient.connectionId)
        {
            return;
        }

        transform.LookAt(enemy.transform.position);

        Vector3 eulerAngles = transform.rotation.eulerAngles;
        eulerAngles.x = 0;
        eulerAngles.z = 0;
        eulerAngles.y -= 180;

        transform.rotation = Quaternion.Euler(eulerAngles);
        //(rtsPlayerv2.un)        if (!(attackSpeed >= attackTimer)) return;
        Debug.Log("Enemy detected");
        Debug.Log($"{enemy.name} detected");

        //   attackSpeed = 0;
    }
}