using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;

    #region  Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }
    [Server]
    private void ServerHandleDie()
    {
//NetworkServer.Destroy(gameObject);
}

    public override void OnStopServer()
    {
       
        health.ServerOnDie -= ServerHandleDie;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(unitPrefab,unitSpawnPoint.position,unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance,connectionToClient);
    }

    #endregion
    #region  Client
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
    

    #endregion
    // Start is called before the first frame update
    void Start()
    { 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
