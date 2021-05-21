using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplayv2 : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText;

    private RTSPlayerv2 player;

 

  private void Start()
  {
      RTSPlayerv2.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
      player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
      ClientHandleResourcesUpdated(player.GetAllResources());
  }

  private void OnDestroy()
  {
      RTSPlayerv2.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;

  }

   
    

   

     public void ClientHandleResourcesUpdated(SyncList<int> resources)
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
 
     private void Update()
    {
        ClientHandleResourcesUpdated(player.resources);
    }
}
