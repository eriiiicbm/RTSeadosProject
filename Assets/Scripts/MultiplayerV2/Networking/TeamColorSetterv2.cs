using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TeamColorSetterv2 : NetworkBehaviour
{
 [SerializeField] private Renderer[] colorRenderers = new Renderer[0];
 [SyncVar(hook   =nameof(HandleTeamColorUpdated))]
 private Color teamColor = new Color();

 #region Server

 public override void OnStartServer()
 {
  RTSPlayerv2 player = connectionToClient.identity.GetComponent<RTSPlayerv2>();
  teamColor = player.GetTeamColor();
        //todo filter colors to avoid the 2 players having the same
 }

 #endregion

 #region Client

 private void HandleTeamColorUpdated(Color oldColor, Color newColor)
 {
  foreach (var renderer in colorRenderers)
  {
   renderer.material.SetColor("_Color",newColor);
  }
 }

 #endregion
}
