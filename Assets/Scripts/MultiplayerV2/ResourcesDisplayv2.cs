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
        // player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
         RTSPlayerv2.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
         player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
         ClientHandleResourcesUpdated(player.GetAllResources());
    }

  

   [Server]
    public override void OnStopServer()
    {
         RTSPlayerv2.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;

    }

    [Client]
    public void ClientHandleResourcesUpdated(List<int> resources)
    {
        if (resources==null)
        {
            return;
        }

        if (player==null)
        {
            return;
        }
        resourcesText.text = $"{resources[0]} I  {resources[1]} X  {resources[2]} W  {resources[3]} S  " +
            $"{player.Trops}/{player.MaxTrops} T  {player.NumHouse}/{player.MaxNumHouse} H";
    }
}
