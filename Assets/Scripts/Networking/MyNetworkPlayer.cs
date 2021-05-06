using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SyncVar] [SerializeField] private string displayName = "Misssing name!";
      
    [SerializeField] private TMP_Text displayText;
    [SerializeField] private Renderer playerRenderer;
    [SyncVar] [SerializeField] private Color displayColor = Color.black;
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;

    }  public void SetDisplayColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;

    }
}
