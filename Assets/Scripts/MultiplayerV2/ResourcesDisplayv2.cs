using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplayv2 : MonoBehaviour
{
    [SerializeField] private List<TMP_Text> resourceTexts;
    private String[] resourcesIcon = { "I", "X", "W", "S" };

    private RTSPlayerv2 player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
            if (player!=null)
            {
                //todo update the cliuenthandleresources to update all the ui
                 ClientHandleResourcesUpdated(player.GetAllResources());
                player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
            }
        }
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(List<int> resources)
    {
        foreach (TMP_Text resourceText in resourceTexts)
        {
            resourceText.text = $"{resources[resourceTexts.IndexOf(resourceText)]} {resourcesIcon[resourceTexts.IndexOf(resourceText)]}";
        }
    }
}
