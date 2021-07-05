using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
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
    private MainMenu mainMenu;
    #region Client

    public override void Start()
    {
        base.Start();
        mainMenu = FindObjectOfType<MainMenu>();
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
        foreach (var player in Players)
        {  player.PutAudios();
            player.PlayListSoundEffect(5,1f,true);
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
        
        foreach (var player in Players)
        {   player.PutAudios();
            player.PlayListSoundEffect(6,1f,true);
        }

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
        if (conn==null)
        {
            return;
        }

        if (conn.identity==null)
        {
            return;
        }

        if (conn.identity.GetComponent<RTSPlayerv2>())
        {
            return;
        }

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
        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)));
        Players.Add(player); 
        //todo change the name to one custom
        if (!mainMenu.useSteam)
        {
         
            player.SetDisplayName($"Player {Players.Count}");

        }
        else
        {

          //  player.SetDisplayName(mainMenu.playerNames[Players.IndexOf(player)]);
            Debug.Log($"{conn.address} + {conn.identity}");
           
        }
      
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
                
                Vector3 position = GetStartPosition().position;
                Debug.Log($"{position} for player {player.name}");
                player.transform.GetChild(0).transform.position = new Vector3(position.x,player.transform.GetChild(0).transform.position.y,position.z-  player.transform.GetChild(0).transform.position.y);
                foreach (var startingUnit in startingUnitsAndBuildings)
                {
                    GameObject unitSpawnerInstance = Instantiate(startingUnit, position + new Vector3(UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f),
                            UnityEngine.Random.Range(0f, 2f)),
                        Quaternion.identity);
                    Debug.Log(unitSpawnerInstance.name);
                    NetworkServer.Spawn(unitSpawnerInstance, player.connectionToClient);
                      player.transform.GetChild(0).transform.position = new Vector3(unitSpawnerInstance.transform.position.x,player.transform.GetChild(0).transform.position.y,unitSpawnerInstance.transform.position.z);

                }  
                player.GetComponent<CameraController>().ReferenceFocus();
            }
            
           
        }
        
    }

    public void StartGame(Gamemode currentGamemode)
    {
        Debug.Log($"Current Gamemode {currentGamemode.ToString()}");
         if (Players.Count<minNumberOfPlayers)
        {
            return;
        }

         switch (currentGamemode)
         {
             case Gamemode.Standard:
                
                 break;
             case  Gamemode.Rush:
                 Time.timeScale = 2;
                 Debug.Log("Timescale ÇÇÇ2");
                 break;
             default:
                 break;
         }
         isGameInProgress = true;
         ServerChangeScene(gameplaySceneName);

    }
    

    #endregion
}