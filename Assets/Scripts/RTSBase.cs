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

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private float currentHealth;

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

    [ContextMenu("Deal damage")]
    [Server]
    public void DealDamage(float damageAmount)
    {
        if (currentHealth == 0)
        {
            return;
        }

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnRTSDie?.Invoke();
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(float oldHealth, float newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
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
        set => currentHealth = value;
    }

    void SetSelected(bool isSelected)
    {
        transform.Find("Highlight").gameObject.SetActive(isSelected);
    }

    public void TakeDamage(RTSBase enemy, float damage)
    {
        // currentHealth = currentHealth - damage;
        //      StartCoroutine(Flasher(GetComponent<Renderer>().material.color));
    }

    IEnumerator Flasher(Color defaultColor)
    {
        var renderer = GetComponent<Renderer>();
        if (currentHealth != 0)
        {
            for (int i = 0; i < 2; i++)
            {
                renderer.material.color = Color.gray;
                yield return new WaitForSeconds(.05f);
                renderer.material.color = defaultColor;
                yield return new WaitForSeconds(.05f);
            }
        }
        else
        {
            Destroy(this.transform.parent.gameObject);
        }
    }

   public void GoToNextState()
    {
        string methodName = this.currentState.ToString() + "State";
        Debug.Log("STATE METHOD NAME " + methodName);
        SendMessage(methodName);
    }

    public virtual IEnumerator IdleState()
    {
        Debug.LogError("OverrideThisMethod before use it");
        yield return new WaitForEndOfFrame();
    }
}