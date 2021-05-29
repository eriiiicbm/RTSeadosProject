
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class GameOverDisplayv2 : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDisplayObject;
    [SerializeField] private TMP_Text winnerNameText;
    void Start()
    {
        GameOverHandlerv2.ClientOnGameOver += ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} HAs Won!";
        gameOverDisplayObject.SetActive(true);
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            
            NetworkManager.singleton.StopClient();
        }
    }
    

    private void OnDestroy()
    {
        GameOverHandlerv2.ClientOnGameOver -= ClientHandleGameOver;
    }
}
