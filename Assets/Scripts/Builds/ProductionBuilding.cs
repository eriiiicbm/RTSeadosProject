using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    List<GameObject> productionUnits;
    float craftTime;
    GameObject curretProduction;
    Queue<GameObject> unitsQueue = new Queue<GameObject>();
    public Vector2 instanceRadius;
    private RTSPlayerv2 player;

    // Start is called before the first frame update
    void Start()
    {

        productionUnits = rtsEntity.ProductionUnits;
        craftTime = rtsEntity.AttackTimer;
        if (rtsEntity.UnitsQueue != null)
        {
            //unitsQueue = rtsEntity.UnitsQueue;
            curretProduction = unitsQueue.Dequeue();
        }

        InvokeRepeating("InstantiateUnit", craftTime, craftTime);
    }

    public void intProduction()
    {
        if (unitsQueue.Count <= 0)
            return;
        if (!player.CheckIfUserHasResources(unitsQueue.Dequeue().GetComponent<Unit>().prices))
            return;
        if (!player.CheckIfUserHasSpaceTrop()) return;

        player.RestPriceToResources(unitsQueue.Dequeue().GetComponent<Unit>().prices);
        curretProduction = unitsQueue.Dequeue();
    }

    public void InstantiateUnit()
    {
        if (curretProduction == null) return;
        if(curretProduction.GetComponent<Unit>().time > 0)
        {
            curretProduction.GetComponent<Unit>().time--;
            return;
        }

        var pos = RandomInsideDonut(instanceRadius);
        Instantiate(curretProduction,
            new Vector3(pos.x + transform.position.x, 0, pos.y + transform.position.z),
            curretProduction.transform.rotation);

        curretProduction = null;
    }

    public void AddUnitToQueue(int unit)
    {
        unitsQueue.Enqueue(productionUnits[unit]);
    }

    public static Vector2 RandomInsideDonut(Vector2 donutRadius)
    {
        var p = Random.Range(donutRadius.x, donutRadius.y);
        var a = Random.Range(0, 360);

        return new Vector2(Mathf.Sin(a * Mathf.Deg2Rad), Mathf.Cos(a * Mathf.Deg2Rad)) * p;
    }
}
