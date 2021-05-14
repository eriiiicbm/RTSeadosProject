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
    [SerializeField] private List<Unit> unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;
    [SerializeField] private GameObject buildingButtonTemplate;
    private Unit currentUnit;
    public Queue<Unit> unitQueue= new Queue<Unit>();
    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    private List<UnitBuildingButtonv2> buttonsList= new List<UnitBuildingButtonv2>();
    [SerializeField] private Transform transformCanvas;
    [SyncVar] private float unitTimer;

    private float progressImageVelocity;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnRTSDie += ServerHandleDie;
        int position = 0;

        foreach(GameObject go in rtsEntity.ProductionUnits)
        {
            unitPrefab.Add(go.GetComponent<Unit>());
        }
        if (rtsEntity.UnitsQueue != null)
        {
            unitQueue = rtsEntity.UnitsQueue;
            currentUnit = rtsEntity.CurretUnit;
        }

        foreach (Unit unit in unitPrefab) {
            GameObject gameObject = Instantiate<GameObject>(buildingButtonTemplate,transformCanvas);
            Debug.Log(gameObject.name +  " name ");
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + position, gameObject.transform.position.y, gameObject.transform.position.z);
            UnitBuildingButtonv2 unitBuildingButtonv2 = gameObject.GetComponent<UnitBuildingButtonv2>();
            unitBuildingButtonv2.SetUnit(unit);
            unitBuildingButtonv2.SetSpawner(this);
            buttonsList.Add(unitBuildingButtonv2);
            position -= 150;

         }
 
    }
    public void AddUnitToTheQueue(Unit unit) {
        unitQueue.Enqueue(unit);
        if (currentUnit == null)
            currentUnit = unit;
        CmdSpawnUnit();

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

    [Command]
    private void CmdSpawnUnit()
    {
        RTSPlayerv2 player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
        Debug.Log ("UnitPrefab is " + currentUnit.name);
        if (!player.CheckIfUserHasResources(currentUnit.prices))
        {
            return;
        }
        if (!player.checkIfUserHasSpaceTrop()) return;
        queuedUnits = unitQueue.Count;
        player.RestPriceToResources(currentUnit.prices);
     }

    [Server]
    private void ProduceUnits()
    {
        if (unitQueue.Count == 0)
        {
            return;
        }
    
        unitTimer += Time.deltaTime;
        if (unitTimer < currentUnit.time)
        {
            return;
        }

        GameObject unitInstance = Instantiate(currentUnit.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;
        Unit unitMovement = unitInstance.GetComponent<Unit>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);
        currentUnit = null;
        queuedUnits = unitQueue.Count;
        unitTimer = 0f;
        if (unitQueue.Count==0)
        {
            return;
        }
        currentUnit = unitQueue.Dequeue();
    }

    #endregion

    #region Client

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
    }

[Client]
public override void Deselect()
    {
        base.Deselect();
        transformCanvas.gameObject.SetActive(false);
    }
[Client]
public override void Select()
    {
        base.Select();
        transformCanvas.gameObject.SetActive(true);
    }
 
}