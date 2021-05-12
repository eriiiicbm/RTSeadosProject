using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : RTSBase
{
    float craftRadius;
    MyEvent onCrafted;
    MyEvent onCraftCompleted;
    GameObject craftCompletedGO;
    GameObject craftUncompletedGO;
    Renderer buildRenderer;
    public int buildTime;
    [SerializeField] private GameObject buildingPreview;
    [SerializeField] private Sprite icon;
    [SerializeField] private List<int> price = new List<int>() { 1,1,1,1};
    [SerializeField] private int id=-1;
    bool _canCraft = false;
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
     } public List<int> GetPrice()
     {
      return price;
     }
    
     #region Server
    
     public override void OnStartServer()
     {
         base.OnStartServer();
    ServerOnBuildingSpawned?.Invoke(this);   
    craftRadius = rtsEntity.CraftRadious;
  //comentado porque si no peta
    craftCompletedGO = transform.Find("FinalEstructure")?.gameObject;
    craftUncompletedGO = transform.Find("plataform")?.gameObject;
    buildTime = rtsEntity.BuildTime;

    if (buildTime <= 0)
        return;
        craftUncompletedGO?.SetActive(false);
        craftCompletedGO?.SetActive(true);
     }
    
     public override void OnStopServer()
     {
         base.OnStopServer();

      ServerOnBuildingDespawned?.Invoke(this); }
    
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
      if (  !hasAuthority)
      {return;
      }
      AuthorityOnBuildingDespawned?.Invoke(this);
     }
    
    
     #endregion
 

    void SetBuild()
    {
        if (buildTime <= 0)
            return;
        craftUncompletedGO.SetActive(true);
        craftCompletedGO.SetActive(false);
    }

    void CraftPoint()
    {
        print("CRAFTING...");
        buildTime--;
        if (buildTime <= 0)
        {
            craftUncompletedGO.SetActive(false);
            craftCompletedGO.SetActive(true);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, craftRadius);
    }

}
