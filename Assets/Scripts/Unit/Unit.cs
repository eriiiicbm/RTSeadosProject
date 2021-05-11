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
     public float expirationVelocity;
    public int[] prices;
    float time;
    [SerializeField] private int resourceCost = 10;
     [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private Targeter targeter;
    [SerializeField] public static event Action<Unit> ServerOnUnitSpawned;
    [SerializeField] public static event Action<Unit> ServerOnUnitDespawned;
    [SerializeField] public static event Action<Unit> AuthorityOnUnitSpawned;
    [SerializeField] public static event Action<Unit> AuthorityOnUnitDespawned;
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] private float chaseRange = 10f;
     
    public int GetResourceCost()
    {
        return resourceCost;
    }

   
    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameOverHandlerv2.ServerOnGameOver += ServerHandleGameOver;

        ServerOnUnitSpawned?.Invoke(this);
        ServerOnDie += ServerHandleDie;

    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandlerv2.ServerOnGameOver -= ServerHandleGameOver;

        ServerOnUnitDespawned?.Invoke(this);
        ServerOnDie -= ServerHandleDie;

        time = rtsEntity.BuildTime;

    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
    [ServerCallback]
    private void Update()
    {
        Targetable target = targeter.GetTarget();

        if (target != null)
        {
            if ((target.transform.position-transform.position).sqrMagnitude> chaseRange*chaseRange)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            else if (navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
            }

            return;
        }

        if (!navMeshAgent.hasPath)
        {
            return;
        }

        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            return;
        }

        navMeshAgent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }
    [Server]
    private void ServerHandleGameOver()
    {
        navMeshAgent.ResetPath();

          
    }
    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();
        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        navMeshAgent.SetDestination(hit.position);
        Debug.Log("Moving");
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

   

    #endregion
    
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
            rtsEntity.Velocity = rtsEntity.Velocity * 0.5f;

            if (rtsEntity.Damage != null) rtsEntity.Damage = rtsEntity.Damage * 0.75f;

            if (rtsEntity.DamageMoral != null) rtsEntity.DamageMoral = rtsEntity.DamageMoral * 0.5f;

            if (rtsEntity.EffectRadious != null) rtsEntity.EffectRadious = rtsEntity.EffectRadious * 0.75f;
            if (rtsEntity.RecoverySpeed != null) rtsEntity.RecoverySpeed = rtsEntity.RecoverySpeed * 0.75f;

            yield return 0;
        }
        yield return new WaitForEndOfFrame();
    }

    public IEnumerator MoveState()
    {
        while (currentState == UnitStates.Follow)
        {

            yield return 0;

        }
        yield return new WaitForEndOfFrame();

    }
}
