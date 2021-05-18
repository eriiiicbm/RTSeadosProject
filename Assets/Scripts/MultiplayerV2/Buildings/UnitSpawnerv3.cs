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
    public List<Unit> unitQueue= new List<Unit>();
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
        Debug.Log("UnitPrefab length" + unitPrefab.Count);
        foreach (Unit unit in unitPrefab) {
            GameObject gameObject = Instantiate<GameObject>(buildingButtonTemplate,transformCanvas);
            Debug.Log(gameObject.name +  " name ");
            gameObject.transform.position = new Vector3(gameObject.transform.position.x + position, gameObject.transform.position.y, gameObject.transform.position.z);
            UnitBuildingButtonv2 unitBuildingButtonv2 = gameObject.GetComponent<UnitBuildingButtonv2>();
            unitBuildingButtonv2.SetUnit(unit);
            unitBuildingButtonv2.SetSpawner(this);
            buttonsList.Add(unitBuildingButtonv2);
            position -= 150;

            Debug.Log("in the for");
        }
 
    }
    public void AddUnitToTheQueue(Unit unit) {
        unitQueue.Add(unit);
        Debug.Log ("uUnitPrice is " + unit.prices[0] +  " " + unit.prices[1] +  " " + unit.prices[2] +  " " + unit.prices[3] +  " "  );

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
        Debug.Log ("UnitPrice is " + currentUnit.prices[0] +  " " + currentUnit.prices[1] +  " " + currentUnit.prices[2] +  " " + currentUnit.prices[3] +  " "  );
        if (!player.CheckIfUserHasSpaceTrop()) return;

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
        if (queuedUnits==0)
        {
            return;
        }
        currentUnit = unitQueue[queuedUnits-1];
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
        Debug.Log("Selected");
    }
 
}