using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSNetworkManagerv2 : NetworkManager
{
    [SerializeField] private GameObject[] startingUnitsAndBuildings;
    [SerializeField] private GameOverHandlerv2 gameOverHandler;
    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public List<RTSPlayerv2> Players { get; } = new List<RTSPlayerv2>();
    private bool isGameInProgress = false;
    public int minNumberOfPlayers=1;
    public string gameplaySceneName="TestRTS";

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();

    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion

    #region Server

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress)
        {
            return;
        }
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayerv2 player = conn.identity.GetComponent<RTSPlayerv2>();
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
Players.Clear();
isGameInProgress = false;
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        RTSPlayerv2 player =
            conn.identity.GetComponent<RTSPlayerv2>();
        Players.Add(player);
        //todo change the name to one custom
        player.SetDisplayName($"Player {Players.Count}");
        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)));
        player.SetPartyOwner(Players.Count==1);
    
    }

    public override void OnServerSceneChanged(string sceneName)
    {
         
        if (SceneManager.GetActiveScene().name.StartsWith(sceneName))
        {
            Debug.Log("valid scene");
            GameOverHandlerv2 gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            foreach (var player in Players)
            {
                foreach (var gameObject in startingUnitsAndBuildings)
                {
                    GameObject unitSpawnerInstance = Instantiate(gameObject, GetStartPosition().position + new Vector3(UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f),
                            UnityEngine.Random.Range(0f, 2f)),
                        Quaternion.identity);
                    Debug.Log(unitSpawnerInstance.name);
                    NetworkServer.Spawn(unitSpawnerInstance, player.connectionToClient);
                }  
            }
           
        }
    }

    public void StartGame()
    {
         if (Players.Count<minNumberOfPlayers)
        {
            return;
        }

         isGameInProgress = true;
         ServerChangeScene(gameplaySceneName);

    }
    

    #endregion
}