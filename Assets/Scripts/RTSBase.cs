using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSBase : NetworkBehaviour
{
    protected UnitStates currentState;
    [SerializeField] protected string entityName;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected string description;
    [SerializeField] public Sprite preview;
    [SerializeField] protected GameObject prefab;
    [SerializeField] public RTSEntity rtsEntity;
    [SerializeField] public Animator animator;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip hitSound;

    [SyncVar(hook = nameof(HandleStatesUpdated))]
    public UnitStates unitStates = UnitStates.Idle;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private float currentHealth;

    [SyncVar] private Color teamColor;
    public event Action ServerOnRTSDie;

    public event Action<float, float> ClientOnHealthUpdated;


    #region Server

    public override void OnStartServer()
    {


        UnitBasev2.ServerOnPlayerDie += ServerHandlePlayerDie;
        currentState = UnitStates.Idle;
        maxHealth = rtsEntity.MaxHealth;

        entityName = rtsEntity.name;
        preview = rtsEntity.Preview;
        prefab = rtsEntity.Prefab;
        currentHealth = maxHealth;

        GoToNextState();
        animator = transform.GetComponentInChildren<Animator>();
        unitStates = UnitStates.Idle;
        teamColor = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>().GetTeamColor();
    }

    public override void OnStopServer()
    {
        UnitBasev2.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId)
        {
            return;
        }

        DealDamage(currentHealth);
    }

    [Server]
    public void DealDamage(float damageAmount)
    {
        if (currentHealth == 0)
        {
            return;
        }

         
        SoundManager._instance.PlaySE(hitSound, 1f);

        currentHealth = Mathf.Min(Mathf.Max(currentHealth - damageAmount, 0), MaxHealth);

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnRTSDie?.Invoke();
        unitStates = UnitStates.Dead;
        SoundManager._instance.PlaySE(deadSound, 1f);
    }
    
    //todo refactor this method
    [Server]
    public void DealDamage(float damageAmount,bool hitSound)
    {
        if (hitSound)
        {
            DealDamage(damageAmount);
            return;
        }
        
        if (currentHealth == 0)
        {
            return;
        }

       
        currentHealth = Mathf.Min(Mathf.Max(currentHealth - damageAmount, 0), MaxHealth);

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnRTSDie?.Invoke();
        unitStates = UnitStates.Dead;
        SoundManager._instance.PlaySE(deadSound, 1f);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(float oldHealth, float newHealth)
    {
//        Debug.Log($"The Health updated is handled {oldHealth}  {newHealth}  {maxHealth} for {gameObject.name} property of  {NetworkClient.connection.identity.GetComponent<RTSPlayerv2>().gameObject.name}");
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
        if (newHealth < oldHealth)
        {
            //todo start flasher 
         }
    }

    private void HandleStatesUpdated(UnitStates oldState, UnitStates newState)
    {
        ResetAllTriggers();
        animator.SetTrigger(newState.ToString());

    }

    #endregion

    public int MaxHealth
    {
        get => maxHealth;
        set => maxHealth = value;
    }

    public float CurrentHealth
    {
        get => currentHealth;
    }

    void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);
    }



  

    public void GoToNextState()
    {
        string methodName = this.currentState.ToString() + "State";
        Debug.Log("STATE METHOD NAME " + methodName);
        SendMessage(methodName);
    }

    public override void OnStartClient()
    {
        maxHealth = rtsEntity.MaxHealth;

        entityName = rtsEntity.name;
        preview = rtsEntity.Preview;
        prefab = rtsEntity.Prefab;
        animator = transform.GetComponentInChildren<Animator>();

    }

    public virtual IEnumerator IdleState()
    {
        Debug.LogWarning("OverrideThisMethod before use it");
        yield return new WaitForEndOfFrame();
    }

    private void ResetAllTriggers()
    {
        if (animator==null)
        {
            return;
        }
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }
}