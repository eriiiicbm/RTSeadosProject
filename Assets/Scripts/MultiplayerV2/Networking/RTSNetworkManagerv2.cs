using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSNetworkManagerv2 : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab;
    [SerializeField] private GameOverHandlerv2 gameOverHandler;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        RTSPlayerv2  player=
        conn.identity.GetComponent<RTSPlayerv2>();
     player.SetTeamColor(new Color(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f)));
        GameObject unitSpawnerInstance   = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
   Debug.Log(unitSpawnerInstance.name);
        NetworkServer.Spawn(unitSpawnerInstance,conn);
        }

    public override void OnServerSceneChanged(string sceneName)
    {
        //todo change the name 
        if (SceneManager.GetActiveScene().name.StartsWith("TestRTS"))
        {
            Debug.Log("valid scene");
            GameOverHandlerv2 gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
