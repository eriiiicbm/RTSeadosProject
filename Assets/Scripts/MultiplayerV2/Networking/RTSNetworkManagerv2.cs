using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class RTSNetworkManagerv2 : NetworkManager
{
    [SerializeField] private GameObject[] startingUnitsAndBuildings;
    [SerializeField] private GameOverHandlerv2 gameOverHandler;

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        RTSPlayerv2 player =
            conn.identity.GetComponent<RTSPlayerv2>();
        player.SetTeamColor(new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)));
        foreach (var gameObject in startingUnitsAndBuildings)
        {
            GameObject unitSpawnerInstance = Instantiate(gameObject, conn.identity.transform.position + new Vector3(UnityEngine.Random.Range(0f, 2f), UnityEngine.Random.Range(0f, 2f),
                    UnityEngine.Random.Range(0f, 2f)),
                conn.identity.transform.rotation);
            Debug.Log(unitSpawnerInstance.name);
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
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