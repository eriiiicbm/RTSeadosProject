using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void Start()
    {
        RTSNetworkManagerv2.ClientOnConnected += HandleClientConnected;
        RTSPlayerv2.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayerv2.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }



    private void OnDestroy()
    {
        RTSNetworkManagerv2.ClientOnConnected -= HandleClientConnected;
        RTSPlayerv2.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        RTSPlayerv2.ClientOnInfoUpdated += ClientHandleInfoUpdated;

    }
 
    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }
    private void ClientHandleInfoUpdated()
    {
        List<RTSPlayerv2> players = ((RTSNetworkManagerv2) NetworkManager.singleton).Players;
        for (int i = 0; i < players.Count; i++)
        {
            playerNameTexts[i].text = players[i].GetDisplayName();
        } 
        for (int i = players.Count; i < playerNameTexts.Length; i++)
        {
            //todo change this string for a textstring ref
            playerNameTexts[i].text = "Waiting For Player...";
        }

        startGameButton.interactable = players.Count >=((RTSNetworkManagerv2) NetworkManager.singleton).minNumberOfPlayers;
    }
    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<RTSPlayerv2>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
