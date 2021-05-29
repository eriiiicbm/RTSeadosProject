using _Project.Scripts;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;
using Random = System.Random;

public class Unit : RTSBase
{
    [SerializeField] public Targetable currentTargeteable;
    [SyncVar] public float expirationVelocity;
    [SyncVar]  public float time;
    [SyncVar] float velocity;

    [SyncVar(hook = nameof(HandleMoralUpdated))]
    public float moral;

    public int id;
    [SyncVar(hook = nameof(HandleMaxMoralUpdated))]
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
    private bool alteratedState;
    public float defaultDistance = 2;
    [SerializeField] private AudioClip movementSound;
    public Targeter GetTargeter()
    {
        return targeter;
    }

    public int GetId()
    {
        return id;
    }

    public void Start()
    {
        base.Start();
        audioList.Insert(2,movementSound);  

        StartStuff();
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
        moral = Mathf.Min(moral + moralSupport, maxMoral);
    }

    public void StartStuff()
    {
        if (navMeshAgent == null)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

        }

        defaultDistance = rtsEntity.DefaultStoppingDistance;
        navMeshAgent.stoppingDistance = defaultDistance;

        if (targeter == null)
        {
            targeter = GetComponent<Targeter>();
        }
         velocity = rtsEntity.Velocity;
        navMeshAgent.speed = velocity;
       
        prices = rtsEntity.Prices;
        time = rtsEntity.BuildTime;
        chaseRange = rtsEntity.AttackRange;
        expirationVelocity = rtsEntity.ExpirationVelocity;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        maxMoral = rtsEntity.Moral;
        moral = maxMoral * 0.5f;
        GameOverHandlerv2.ServerOnGameOver += ServerHandleGameOver;
//todo assign ids
        //       id = UnityEngine.Random.Range(0, 999999999);

        ServerOnUnitSpawned?.Invoke(this);

        ServerOnRTSDie += ServerHandleDie;

        StartStuff();
        navMeshAgent.stoppingDistance = rtsEntity.AttackRange;

        playerv2 = connectionToClient.identity.GetComponent<RTSPlayerv2>();

        StartCoroutine(nameof(ExpirationEffect));

        if (playerv2 != null)
        {
            return;
        }

        Debug.LogWarning("Player is null onstart");
        DealMoralDamage(maxMoral/2);
        HandleMoralUpdated(0,maxMoral/2);

    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandlerv2.ServerOnGameOver -= ServerHandleGameOver;
        ServerOnUnitDespawned?.Invoke(this);

        ServerOnRTSDie -= ServerHandleDie;
        

    }

    [Server]
    private void ServerHandleDie()
    {
      playerv2.Trops--;
    }

  
    protected Targetable target;
    [ServerCallback]
    public virtual void Update()
    {
      NavMeshToTarget();
    }

[Server]
private void NavMeshToTarget()
    {
         target = targeter?.GetTarget();

        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            else if (navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
                if (unitStates!=UnitStates.Attack)
                {
                 
                    unitStates = UnitStates.Idle;   
                }
            }

            return;
        }

        if (!navMeshAgent.hasPath)
        {
            return;
        }
        PlayListSoundEffect(2,1,false);
    // PlaySEIfNotPlaying(movementSound,1f);
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
            return;
        }

        navMeshAgent.ResetPath();
        
        if (unitStates!=UnitStates.Attack)
        {
                 
            unitStates = UnitStates.Idle;   
        }
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

         
            unitStates = UnitStates.Walk;
         PlayListSoundEffect(2,1,false);
        navMeshAgent.SetDestination(hit.position);
        Debug.Log("Moving");
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AuthorityOnUnitSpawned?.Invoke(this);
        Debug.Log("Auth");
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
        if (isServer)
        {
         
            StartCoroutine(nameof(MoralEfect));

        }} 
    private void HandleMaxMoralUpdated(float oldMaxMoral, float newMaxMoral)
    {
        ClientOnMoralUpdated?.Invoke(moral, newMaxMoral);
    }


    IEnumerator ExpirationEffect()
    {
        while (true)
        {
            DealDamage(1 * expirationVelocity,false);
            yield return new WaitForSeconds(1);
        }
    }
     IEnumerator MoralEfect()
    { 
        //todo aura
        if (GetComponent<Unit>() == null) yield return 0;

        PassiveAbility passiveAbility = GetComponent<PassiveAbility>();
        UnitCombat unitCombat = GetComponent<UnitCombat>();
        MoralDamage moralDamage = GetComponent<MoralDamage>();

        if (moral < maxMoral * 0.25 && !alteratedState)
        {
            alteratedState = true;

            velocity = rtsEntity.Velocity * 0.5f;
            navMeshAgent.speed = velocity;

            if (unitCombat != null) unitCombat.damage = rtsEntity.Damage * 0.25f;

            if (moralDamage != null)
                moralDamage.damageMoral = rtsEntity.DamageMoral * 0.5f;

            if (passiveAbility != null)
            {
                passiveAbility.efectRadius = rtsEntity.EffectRadious * 0.75f;
                passiveAbility.recoverySpeed = rtsEntity.RecoverySpeed * 0.90f;
            }

            yield return 0;
        }

        if (moral < maxMoral * 0.25 && moral < maxMoral * 0.75 && alteratedState)
        {
            alteratedState = false;

            velocity = rtsEntity.Velocity;
            navMeshAgent.speed = velocity;

            if (unitCombat != null) unitCombat.damage = rtsEntity.Damage;

            if (moralDamage != null)
                moralDamage.damageMoral = rtsEntity.DamageMoral;

            if (passiveAbility != null)
            {
                passiveAbility.efectRadius = rtsEntity.EffectRadious;
                passiveAbility.recoverySpeed = rtsEntity.RecoverySpeed;
            }

            yield return 0;
        }

        if (moral > maxMoral * 0.75 && !alteratedState)
        {
            alteratedState = true;

            velocity = rtsEntity.Velocity * 1.5f;
            navMeshAgent.speed = velocity;

            if (unitCombat != null) unitCombat.damage = rtsEntity.Damage * 1.75f;

            if (moralDamage != null)
                moralDamage.damageMoral = rtsEntity.DamageMoral * 1.5f;

            if (passiveAbility != null)
            {
                passiveAbility.efectRadius = rtsEntity.EffectRadious * 1.25f;
                passiveAbility.recoverySpeed = rtsEntity.RecoverySpeed * 1.10f;
            }

            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator MoveState()
    {
        while (unitStates == UnitStates.Walk)
        {
            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }

   
}