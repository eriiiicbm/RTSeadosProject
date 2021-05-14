using _Project.Scripts;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;

public class Unit : RTSBase
{

    public Transform currentTarget;
    public float expirationVelocity;
    public float time;
    float velocity;
    [SyncVar(hook = nameof(HandleMoralUpdated))]
    public float moral;
    public float maxMoral;
    public List<int> prices; 
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private Targeter targeter;
    [SerializeField] public static event Action<Unit> ServerOnUnitSpawned;
    [SerializeField] public static event Action<Unit> ServerOnUnitDespawned;
    [SerializeField] public static event Action<Unit> AuthorityOnUnitSpawned;
    [SerializeField] public static event Action<Unit> AuthorityOnUnitDespawned;
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] private float chaseRange = 10f;
    public event Action<float, float> ClientOnMoralUpdated;
    public event Action ServerOnLostMoral;
   
    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    [ContextMenu("Deal damage")]
    [Server]
    public void DealMoralDamage(float damageAmount)
    {
        moral = Mathf.Max(moral - damageAmount, 0);

        //ServerOnLostMoral?.Invoke();
    }

    [ContextMenu("Deal support")]
    [Server]
    public void DealMoralSupport(float moralSupport)
    {
        moral = Mathf.Max(moral + moralSupport, maxMoral);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameOverHandlerv2.ServerOnGameOver += ServerHandleGameOver;

        ServerOnUnitSpawned?.Invoke(this);
        ServerOnDie += ServerHandleDie;
        velocity = rtsEntity.Velocity;
        navMeshAgent.speed = velocity;
        maxMoral = rtsEntity.Moral;
        moral = maxMoral * 0.5f;
        prices = rtsEntity.Prices;

        connectionToClient.identity.GetComponent<RTSPlayerv2>().addTrops();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandlerv2.ServerOnGameOver -= ServerHandleGameOver;

        ServerOnUnitDespawned?.Invoke(this);
        ServerOnDie -= ServerHandleDie;

    }
    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);

        connectionToClient.identity.GetComponent<RTSPlayerv2>().Trops--;
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
    private void HandleMoralUpdated(float oldMoral, float newMoral)
    {
        ClientOnMoralUpdated?.Invoke(newMoral, maxMoral);
        StartCoroutine(nameof(MoralEfect));
    }

    void SetNewTarget(Transform target)
    {
        currentTarget = target;

    }
    void ExpirationEffect()
    {
        Destroy(this.transform.parent.gameObject);
    }

    IEnumerator MoralEfect()
    {
        if (GetComponent<Unit>() == null) yield return 0;
        if (moral < maxMoral *0.25)
        {
            velocity = rtsEntity.Velocity * 0.5f;

            if(GetComponent<UnitCombat>() != null) GetComponent<UnitCombat>().damage = rtsEntity.Damage * 0.75f;

            if (GetComponent<MoralDamage>() != null) GetComponent<MoralDamage>().damageMoral = rtsEntity.DamageMoral * 0.5f;

            if (GetComponent<PasiveHability>() != null)
            {
                GetComponent<PasiveHability>().efectRadius = rtsEntity.EffectRadious * 0.75f;
                GetComponent<PasiveHability>().recoverySpeed = rtsEntity.RecoverySpeed * 0.75f;
            }
            
            yield return 0;
        }

        if (moral > maxMoral * 0.75)
        {
            velocity = rtsEntity.Velocity * 1.5f;

            if (GetComponent<UnitCombat>() != null) GetComponent<UnitCombat>().damage = rtsEntity.Damage * 1.75f;

            if (GetComponent<MoralDamage>() != null) GetComponent<MoralDamage>().damageMoral = rtsEntity.DamageMoral * 1.5f;

            if (GetComponent<PasiveHability>() != null)
            {
                GetComponent<PasiveHability>().efectRadius = rtsEntity.EffectRadious * 1.75f;
                GetComponent<PasiveHability>().recoverySpeed = rtsEntity.RecoverySpeed * 1.75f;
            }

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
