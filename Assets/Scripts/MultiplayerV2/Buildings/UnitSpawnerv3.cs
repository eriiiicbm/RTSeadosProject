using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UnitSpawnerv3 : Building, IPointerClickHandler
{
    [SerializeField] private GameObject instantiatedCanvas;
    [SerializeField] private List<Unit> unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private float spawnMoveRange = 7;
    [SyncVar]
    [SerializeField] private float unitSpawnDuration = 5f;
    [SerializeField] private GameObject buildingButtonTemplate;
    private Unit currentUnit;
    public List<Unit> unitQueue = new List<Unit>();
    private RTSPlayerv2 player;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;

    private List<UnitBuildingButtonv2> buttonsList = new List<UnitBuildingButtonv2>();
    private Transform transformCanvas;
    [SyncVar] private float unitTimer;
    private float progressImageVelocity;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnRTSDie += ServerHandleDie;

        Debug.Log("UnitPrefab length" + unitPrefab.Count);
        player =connectionToClient.identity.GetComponent<RTSPlayerv2>();
    }

    private void Start()
    {
     SpawnButtons();

    }

    public void SpawnButtons()
    {
        int position = 0;
        GameObject canvas = Instantiate<GameObject>(instantiatedCanvas);
        transformCanvas = canvas.transform;

        foreach (Unit unit in unitPrefab)
        {
            GameObject gameObject = Instantiate<GameObject>(buildingButtonTemplate);
            Debug.Log(gameObject.name + " name ");
            var position1 = gameObject.transform.position;
            position1 = new Vector3(position1.x + position,
                position1.y, position1.z);
            gameObject.transform.position = position1;
            UnitBuildingButtonv2 unitBuildingButtonv2 = gameObject.GetComponent<UnitBuildingButtonv2>();
            unitBuildingButtonv2.SetUnit(unit);
            unitBuildingButtonv2.SetSpawner(this);
            buttonsList.Add(unitBuildingButtonv2);
            position -= 150;
            gameObject.transform.SetParent(canvas.transform);
            gameObject.transform.localScale =  Vector3.one;

            Debug.Log("in the for");
            transformCanvas = canvas.transform;

        }
    }
     public void AddUnitToTheQueue(int id)
    {
       
        Unit unit = player.FindUnitById(id);
        Debug.Log("uUnitPrice is " + unit.prices[0] + " " + unit.prices[1] + " " + unit.prices[2] + " " +
                  unit.prices[3] + " ");
        if (currentUnit == null)
        {
         
            currentUnit = unit;   
            unitSpawnDuration = currentUnit.rtsEntity.BuildTime;
        }
        unitQueue.Add(unit);
        Debug.Log("UnitPrefab is " + currentUnit.name);
        Debug.Log("UnitPricecmd is " + currentUnit.prices[0] + " " + currentUnit.prices[1] + " " +
                  currentUnit.prices[2] + " " + currentUnit.prices[3] + " ");
       

        queuedUnits++;
    //    player.RestPriceToResources(currentUnit.prices);
        Debug.Log("NO BOOM");
        player.RestPriceToResources(unit.prices); 
       // CmdSpawnUnit();
     //   CmdSpawnUnit(); // < CmdSpawnUnit();
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerOnRTSDie -= ServerHandleDie;
    }

     public void CmdSpawnUnit()
    {
        Debug.Log("UnitPrefab is " + currentUnit.name);
        Debug.Log("UnitPricecmd is " + currentUnit.prices[0] + " " + currentUnit.prices[1] + " " +
                  currentUnit.prices[2] + " " + currentUnit.prices[3] + " ");
        if (!player.CheckIfUserHasSpaceTrop()) return;

        if (!player.CheckIfUserHasResources(currentUnit.prices))
        {
            return;
        }

        queuedUnits++;
        player.RestPriceToResources(currentUnit.prices);
        Debug.Log("NO BOOM");
    }


    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0)
        {
            return;
        }

        unitTimer += Time.deltaTime;
        if (unitTimer < unitSpawnDuration)
        {
            return;
        }

        Debug.Log("Producting");
        GameObject unitInstance = Instantiate(currentUnit.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;
        Unit unitMovement = unitInstance.GetComponent<Unit>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);
        unitQueue.Remove(currentUnit);
        currentUnit = null;
        queuedUnits--;
        unitTimer = 0f;
        if (queuedUnits == 0)
        {
            return;
        }

        currentUnit = unitQueue[queuedUnits - 1];
        unitSpawnDuration = currentUnit.rtsEntity.BuildTime;
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;
        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress,
                ref progressImageVelocity, 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!hasAuthority)
        {
            return;
        }
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }

        /*   if (player == null)
           {
               if (NetworkClient.connection.identity == null)
               {
                   Debug.LogError("You dont have identity");
               }
               else
               {
                   Debug.Log("Identity name" + NetworkClient.connection.identity.name);
               }
   
               player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
               if (player == null)
               {
                   Debug.LogWarning("Try 1 failed");
                   player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
               }
   
               if (player == null)
               {
                   Debug.LogWarning("Try 2 failed");
                   player = connectionToServer.identity.GetComponent<RTSPlayerv2>();
               }
   
               return;
           }
           else
           {
               Debug.Log("LIFE ");
               return;
           }
   
           Debug.LogError("The player is more null that your desires of live");
      */
    }

    [Client]
    public override void Deselect()
    { if (!hasAuthority)
        {
            return;
        }
        base.Deselect();
        transformCanvas.gameObject.SetActive(false);
    }

    [Client]
    public override void Select()
    {
        if (!hasAuthority)
        {
            return;
        }
        base.Select();

        if (!base.builded) return;

        transformCanvas.gameObject.SetActive(true);
        Debug.Log("Selected");
    }
}