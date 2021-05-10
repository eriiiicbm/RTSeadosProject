using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : RTSBase
{
    float craftRadius;
    public MyEvent onCrafted;
    public MyEvent onCraftCompleted;

    GameObject craftCompletedGO;
    GameObject craftUncompletedGO;
    Renderer buildRenderer;
    public int buildTime;

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
    void Start()
    {

        craftRadius = rtsEntity.CraftRadious;
        craftCompletedGO = rtsEntity.Prefab.transform.Find("FinalEstructure").gameObject;
        craftUncompletedGO = rtsEntity.Prefab.transform.Find("plataform").gameObject;
        buildRenderer = craftCompletedGO.GetComponent<MeshRenderer>();
        buildTime = rtsEntity.BuildTime;

        if (buildTime <= 0)
            return;
        craftUncompletedGO.SetActive(false);
        craftCompletedGO.SetActive(true);
    }

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
            onCraftCompleted.Invoke();
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
