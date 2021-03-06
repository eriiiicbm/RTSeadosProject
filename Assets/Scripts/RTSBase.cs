using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSBase : NetworkBehaviour
{
     [SerializeField] protected string entityName;
    [SerializeField] protected int maxHealth;
    [SerializeField] protected string description;
    [SerializeField] public Sprite preview;
    [SerializeField] protected GameObject prefab;
    [SerializeField] public RTSEntity rtsEntity;
    [SerializeField] public Animator animator;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip healSound;

    [SyncVar(hook = nameof(HandleStatesUpdated))]
    public UnitStates unitStates = UnitStates.Idle;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private float currentHealth;

     public event Action ServerOnRTSDie;
     protected RTSPlayerv2 playerv2;

    public event Action<float, float> ClientOnHealthUpdated;
   [HideInInspector] public List<AudioClip> audioList= new List<AudioClip>( );
    public virtual  void Start()
    {
audioList.Insert(0,deadSound); 
audioList.Insert(1,hitSound);  
audioList.Insert(2,healSound);  


    }

    #region Server

    public override void OnStartServer()
    {


        UnitBasev2.ServerOnPlayerDie += ServerHandlePlayerDie;
        animator = transform.GetComponentInChildren<Animator>();
        unitStates = UnitStates.Idle;
        maxHealth = rtsEntity.MaxHealth;

        entityName = rtsEntity.name;
        preview = rtsEntity.Preview;
        prefab = rtsEntity.Prefab;
        currentHealth = maxHealth;

        GoToNextState();
        playerv2 = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();

     }

    public override void OnStopServer()
    {
        UnitBasev2.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        playerv2 = connectionToClient.identity.GetComponent<RTSPlayerv2>();

         
        if (playerv2.connectionToClient.connectionId != connectionId)
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


        if (damageAmount<=0)
        {
            PlayListSoundEffect(2,1,false);
        }else{
        PlayListSoundEffect(1,1,true);
        } currentHealth = Mathf.Min(Mathf.Max(currentHealth - damageAmount, 0), MaxHealth);

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnRTSDie?.Invoke();
        unitStates = UnitStates.Dead;
        StartCoroutine(nameof(DeadAnim));

        PlayListSoundEffect(0,1,true);
    }

    //todo refactor this method
    [Server]
    public void DealDamage(float damageAmount, bool hitSound)
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
        StartCoroutine(nameof(DeadAnim));
        PlayListSoundEffect(0,1,true);
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
            //shake
        }
    }

    private void HandleStatesUpdated(UnitStates oldState, UnitStates newState)
    {
        if (animator == null)
        {
            return;
        }

        ResetAllTriggers();
        animator.SetTrigger(newState.ToString());

    }

    #endregion

    #region ClientSound

   
[ClientRpc]
    public void PlayListSoundEffect(int position,float pitch,bool overwrite)
    {
        if (position>=audioList.Count || position <0)
        {
            return;
        }
        if (overwrite)
        {
            SoundManager._instance.PlaySE(audioList[position],pitch);
      
        }
        else
        {
            SoundManager._instance.PlaySEIfNotPlaying(audioList[position],pitch);
        }
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
    IEnumerator DeadAnim()
    {
        if (animator==null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        animator.Play("Dead");

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        animator.Play("Dead anim played");

        NetworkServer.Destroy(gameObject);

        
    }
    void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);
    }



  

    public void GoToNextState()
    {
        string methodName = this.unitStates.ToString() + "State";
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
       
        foreach (var param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                animator.ResetTrigger(param.name);
            }
        }
    }

    public virtual void Update()
    {
        
    }
}