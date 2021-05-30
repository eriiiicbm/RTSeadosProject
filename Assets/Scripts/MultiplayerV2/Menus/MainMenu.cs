using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] public bool useSteam = false;
    public TMP_InputField text;
    public   List<string> playerNames;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        if (!useSteam)
        {
            return;
        }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered=Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);
        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult!=EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
        }
        NetworkManager.singleton.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"HostAddress",SteamUser.GetSteamID().ToString());
      //  text.text = $"{SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"HostAddress")}    user id {SteamUser.GetSteamID().ToString()}  csteamid {new CSteamID(callback.m_ulSteamIDLobby).ToString()}  :F {new CSteamID(callback.m_ulSteamIDLobby).m_SteamID} ";
        text.text = $"{SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),"HostAddress")} ";
        CSteamID ds;
        
        playerNames.Add(SteamFriends.GetPersonaName() );
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEnter(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
        {
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");
        Debug.Log($"host address {hostAddress}");
        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();
            landingPagePanel.SetActive(false);
    }
}