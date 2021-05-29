using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mirror;
using UnityEngine;

public class TeamColorSetterv2 : NetworkBehaviour
{
 [SerializeField] private Renderer[] colorRenderers = new Renderer[0];
 [SyncVar(hook   =nameof(HandleTeamColorUpdated))]
 private Color teamColor = new Color();

 private RTSPlayerv2 player;
 #region Server

 public override void OnStartServer()
 {  
  
        //todo filter colors to avoid the 2 players having the same
 }
[ServerCallback]
 private void Update()
 {
  if (player != null) return;
  if (connectionToClient == null) return;
  if (connectionToClient.identity == null) return;
  player =connectionToClient.identity.GetComponent<RTSPlayerv2>();
  teamColor = player.GetTeamColor();
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
