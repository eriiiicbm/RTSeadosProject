using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using UnityEngine;

public class GameOverHandlerv2 : NetworkBehaviour
{
  public static event Action  ServerOnGameOver;
  public static event Action<string> ClientOnGameOver;
  private List<UnitBasev2> bases = new List<UnitBasev2>();
  #region Server

  public override void OnStartServer()
  {
    UnitBasev2.ServerOnBaseSpawned += SeverHandleBaseSpawned;
    UnitBasev2.ServerOnBaseDespawned += SeverHandleBaseDespawned;
  }

  [Server]
  private void SeverHandleBaseSpawned(UnitBasev2 unitBase)
  {
bases.Add(unitBase);  }

  public override void OnStopServer()
  {

    UnitBasev2.ServerOnBaseSpawned -= SeverHandleBaseSpawned;
    UnitBasev2.ServerOnBaseDespawned -= SeverHandleBaseDespawned;

  }

  [Server]
  private void SeverHandleBaseDespawned(UnitBasev2 unitBase)
  {
    bases.Remove(unitBase);
    if (bases.Count!=1)
    {
      return;
    }

    int playerId = bases[0].connectionToClient.connectionId;
    RpcGameOver($"Player {playerId}");
    ServerOnGameOver?.Invoke();
    Debug.Log("Game Over");
  }

  #endregion

  #region Client

  [ClientRpc]
  private void RpcGameOver(string winner)
  {
    ClientOnGameOver?.Invoke(winner);
    Debug.Log("Winner invoked");
  }

  #endregion
}
