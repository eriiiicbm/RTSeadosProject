using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Building : RTSBase
{
   [SyncVar] float craftRadius;
    GameObject craftCompletedGO;
    GameObject craftUncompletedGO;
    Renderer buildRenderer;
   
    [SyncVar] public int buildTime;
    [SerializeField] private GameObject buildingPreview;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<int> price = new List<int>() { 1,1,1,1};
    [SerializeField] private int id=-1;
    bool _canCraft = false;
    [SerializeField] private UnityEvent onSelected;
    [SerializeField] private UnityEvent onDeselected;
    [SerializeField] private AudioClip buildedFinishedSound;
    [SyncVar(hook = nameof(HandleBuildedUpdated))]  public bool builded;

    public bool canCraft
    {
        get
        {
            return _canCraft;
        }
        set
        {
            _canCraft = value;
            buildRenderer.material.color = _canCraft ? Color.white : Color.red;
        }
    }
    public int collidersCount
    {
        get
        {
            var Sphere = Physics.SphereCastAll(transform.position, craftRadius, -Vector3.up);
            return Sphere.Length;
        }
    }

    private void Start()
    {
        base.Start();
        craftCompletedGO = transform.Find("FinalEstructure")?.gameObject;
        craftUncompletedGO = transform.Find("Platform")?.gameObject;
        audioList.Insert(2,buildedFinishedSound);
    }

    [SerializeField] public  static  event Action<Building> ServerOnBuildingSpawned;
     [SerializeField] public  static  event Action<Building> ServerOnBuildingDespawned;
     [SerializeField] public  static  event Action<Building> AuthorityOnBuildingSpawned;
     [SerializeField] public  static  event Action<Building> AuthorityOnBuildingDespawned;
    
     public GameObject getBuildingPreview()
     {
      return buildingPreview;
     }public Sprite GetIcon()
     {
      return icon;
     }
     public int GetId()
     {
      return id;
     } 

    public List<int> GetPrice()
    {
      return price;
     }
       public void  SetPrice(List<int> newPrices)
       {
           price = newPrices;
       }
    
     #region Server
    
     public override void OnStartServer()
     {
         base.OnStartServer();
        ServerOnRTSDie += ServerHandleDie;
        ServerOnBuildingSpawned?.Invoke(this);
        price = rtsEntity.Prices;
        craftRadius = rtsEntity.CraftRadious;
        craftCompletedGO = transform.Find("FinalEstructure")?.gameObject;
        craftUncompletedGO = transform.Find("Platform")?.gameObject;

        StartCoroutine(nameof(InConstuction));

        buildTime = rtsEntity.BuildTime;
        DealDamage(maxHealth-1);
        SetBuild();
     }
    
     public override void OnStopServer()
     {
         base.OnStopServer();

        ServerOnRTSDie -= ServerHandleDie;

        ServerOnBuildingDespawned?.Invoke(this); }

    [Server]
    private void ServerHandleDie()
    { 
        if (GetComponent<Fridge>() == null) return;
        connectionToClient.identity.GetComponent<RTSPlayerv2>().DeleteHouse();
    }
    
    
    [Server] 
    void SetBuild()
    {
        builded = buildTime < 0;
    }

    #endregion

    #region Client

    public override void OnStartAuthority()
     {
         base.OnStartAuthority();
      AuthorityOnBuildingSpawned?.Invoke(this);


 
     }
    
     public override void OnStopClient()
     {
        base.OnStopClient();
        if (!hasAuthority) return;
        AuthorityOnBuildingDespawned?.Invoke(this);
     }
     [Client]
     public virtual void Select()
     {
         if (!hasAuthority)
         {
             return;
         }
         
         onSelected?.Invoke();
     }

     [Client]
     public  virtual  void Deselect()
     {
         if (!hasAuthority)
         {
             return;
         }
         onDeselected?.Invoke();

     }
    #endregion


    void CraftPoint()
    {
        print("CRAFTING...");
        buildTime--;
    //todo pass the correct number to dealdamage

        DealDamage(-250);

        if (!(base.CurrentHealth >= base.MaxHealth)) return;
        builded = true;
        PlayListSoundEffect(8,1f,true);
        StartCoroutine(nameof(Builded));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, craftRadius);
    }

    public IEnumerator InConstuction()
    {
        while (!builded)
        {
            craftUncompletedGO.SetActive(true);
            craftCompletedGO.SetActive(false);

            yield return 0;
        }

        yield return new WaitForEndOfFrame();
    }

    public IEnumerator Builded()
    {
        Debug.Log("builded" + name);

        craftUncompletedGO.SetActive(false);
        craftCompletedGO.SetActive(true);
        animator = craftCompletedGO.GetComponent<Animator>();

        Fridge fridge = GetComponent<Fridge>();
        if (fridge != null)
        {
            fridge.enabled = true;
            connectionToClient.identity.GetComponent<RTSPlayerv2>().MaxTrops += 3;
        }

        yield return 0;
    }

    public void HandleBuildedUpdated(bool oldBuilding, bool newBuilding)
    {
        craftCompletedGO = transform.Find("FinalEstructure")?.gameObject;
        craftUncompletedGO = transform.Find("Platform")?.gameObject;
        if (!newBuilding) return;
        craftUncompletedGO.SetActive(false);
        craftCompletedGO.SetActive(true);
        
        animator = craftCompletedGO.GetComponent<Animator>();
    }
}
