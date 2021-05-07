using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    // Start is called before the first frame update
    [SyncVar(hook = nameof(HandleHealthUpdated))] private int currentHealth;
    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;
    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
       

        currentHealth  = Mathf.Max(currentHealth-damageAmount,0);
        if (currentHealth!=0)
        {
            return;
        }
        ServerOnDie?.Invoke();
        Debug.Log("Y se murio");
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth,maxHealth);
    }


    #endregion
}
