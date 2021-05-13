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
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;
    [SerializeField] private GameObject buildingButtonTemplate;
    private Unit currentUnit;
    public List<Unit> unitQueue= new List<Unit>();
    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    private List<UnitBuildingButtonv2> buttonsList;
    [SyncVar] private float unitTimer;

    private float progressImageVelocity;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnDie += ServerHandleDie;
        int position = 0;
        foreach (Unit unit in unitPrefab) {
            GameObject gameObject = Instantiate<GameObject>(buildingButtonTemplate);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + position, gameObject.transform.position.y, gameObject.transform.position.z);
            UnitBuildingButtonv2 unitBuildingButtonv2 = gameObject.GetComponent<UnitBuildingButtonv2>();
            unitBuildingButtonv2.SetUnit(unit);
            buttonsList.Add(unitBuildingButtonv2.GetComponent<UnitBuildingButtonv2>());
            position += 20;

         }
 
    }
    public void AddUnitToTheQueue(Unit unit) {
        unitQueue.Add(unit);
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
        ServerOnDie -= ServerHandleDie;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits == maxUnitQueue)
        {
            return;
        }

        RTSPlayerv2 player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
        Debug.Log("UnitPrefab is " + currentUnit.name);
        if (!player.CheckIfUserHasResources(currentUnit.prices))
        {
            return;
        }

        queuedUnits++;
        player.RestPriceToResources(currentUnit.prices);
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

         CmdSpawnUnit();
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
}