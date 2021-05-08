using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
  public static event Action<string> ClientOnGameOver;
  private List<UnitBase> bases = new List<UnitBase>();
  #region Server

  public override void OnStartServer()
  {
    UnitBase.ServerOnBaseSpawned += SeverHandleBaseSpawned;
    UnitBase.ServerOnBaseDespawned += SeverHandleBaseDespawned;
  }

  [Server]
  private void SeverHandleBaseSpawned(UnitBase unitBase)
  {
bases.Add(unitBase);  }

  public override void OnStopServer()
  {

    UnitBase.ServerOnBaseSpawned -= SeverHandleBaseSpawned;
    UnitBase.ServerOnBaseDespawned -= SeverHandleBaseDespawned;

  }

  [Server]
  private void SeverHandleBaseDespawned(UnitBase unitBase)
  {
    bases.Remove(unitBase);
    if (bases.Count!=1)
    {
      return;
    }

    int playerId = bases[0].connectionToClient.connectionId;
    RpcGameOver($"Player {playerId}");
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
