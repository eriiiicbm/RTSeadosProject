using _Project.Scripts;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Unit : RTSBase
{

    public Transform currentTarget;
    public NavMeshAgent navAgent;
    public float expirationVelocity;
    public int[] prices;
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private UnitMovement unitMovement;
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private Targeter targeter;
    [SerializeField] public static event Action<Unit> ServerOnUnitSpawned;
    [SerializeField] public static event Action<Unit> ServerOnUnitDespawned;
    [SerializeField] public static event Action<Unit> AuthorityOnUnitSpawned;
    [SerializeField] public static event Action<Unit> AuthorityOnUnitDespawned;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private float chaseRange = 10f;
     
    public int GetResourceCost()
    {
        return resourceCost;
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }
    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;

        ServerOnUnitSpawned?.Invoke(this);
        ServerOnDie += ServerHandleDie;

    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;

        ServerOnUnitDespawned?.Invoke(this);
        ServerOnDie -= ServerHandleDie;

    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    #endregion
    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        base.OnStopAuthority();

        if (!hasAuthority)
        {
            return;
        }
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if (!hasAuthority)
        {
            return;
        }
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if (!hasAuthority)
        {
            return;
        }
        onDeselected?.Invoke();

    }

    void MoveUnit(Vector3 dest)
    {
        currentTarget = null;
        navAgent.destination = dest;
    }
    void SetNewTarget(Transform target)
    {
        currentTarget = target;

    }
    void ExpirationEffect()
    {
        Destroy(this.transform.parent.gameObject);
    }

    IEnumerator moralEfect()
    {
        while (rtsEntity.Moral > 5)
        {
            switch (currentState)
            {
                case EntityType.
            }

            yield return 0;
        }
        yield return new WaitForEndOfFrame();
    }

}
