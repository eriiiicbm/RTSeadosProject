using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplayv2 : NetworkBehaviour
{
    [SerializeField] private TMP_Text resourcesText;

    private RTSPlayerv2 player;
    // Start is called before the first frame update
  [Server]
    public override void OnStartServer()
    {
         player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
         RTSPlayerv2.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;

    }

 
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
       RTSPlayerv2.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    
        }
/*
       // if (player!=null)
        //    {
        //    {
                //todo update the cliuenthandleresources to update all the ui
            //     ClientHandleResourcesUpdated(player.GetAllResources());
           // }
           */
     
    }

   [Server]
    public override void OnStopServer()
    {
         RTSPlayerv2.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;

    }

    [Client]
    private void ClientHandleResourcesUpdated(List<int> resources)
    {
        Debug.Log( $"{resources[0]} I  {resources[1]} X  {resources[2]} W  {resources[3]} S  " +
                   $"{player.Trops}/{player.MaxTrops} T  {player.NumHouse}/{player.MaxNumHouse} H");
        resourcesText.text = $"{resources[0]} I  {resources[1]} X  {resources[2]} W  {resources[3]} S  " +
            $"{player.Trops}/{player.MaxTrops} T  {player.NumHouse}/{player.MaxNumHouse} H";
    }
}
