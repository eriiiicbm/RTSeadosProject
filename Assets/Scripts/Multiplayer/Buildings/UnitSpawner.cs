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

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue=5;
    [SerializeField] private float spawnMoveRange=7;
    [SerializeField] private float unitSpawnDuration=5f;
    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
     private float unitTimer;

    private float progressImageVelocity;
    
    #region  Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
 NetworkServer.Destroy(gameObject);
}

    public override void OnStopServer()
    {
       
        health.ServerOnDie -= ServerHandleDie;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits== maxUnitQueue)
        {
            return;
        }

        RTSPlayer player= connectionToClient.identity.GetComponent<RTSPlayer>();
        if (player.GetResources() < unitPrefab.GetResourceCost())
        {
            return;
        }

        queuedUnits++;
        player.SetResources(player.GetResources()-unitPrefab.GetResourceCost());

    }
    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits==0)
        {return;}

        unitTimer += Time.deltaTime;
        if (unitTimer<unitSpawnDuration)
        {
            return;
        }
        GameObject unitInstance = Instantiate(unitPrefab.gameObject,unitSpawnPoint.position,unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance,connectionToClient);
        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position+spawnOffset);
        queuedUnits--;
        unitTimer = 0f;
    }
    
    #endregion
    #region

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;
        if (newProgress<unitProgressImage.fillAmount)
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
        { return;}

        if (!hasAuthority)
        {
            return;
        }
        CmdSpawnUnit();
        
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits,int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }

    #endregion
    // Start is called before the first frame update
    void Start()
    { 
    }

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
