using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    GameObject[] productionUnits;
    float craftTime;
    GameObject curretProduction;
    Queue<GameObject> unitsQueue = new Queue<GameObject>();
    public Vector2 instanceRadius;

    // Start is called before the first frame update
    void Start()
    {

        productionUnits = rtsEntity.ProductionUnits;
        craftTime = rtsEntity.AttackTimer;
        unitsQueue = rtsEntity.UnitsQueue;
        curretProduction = unitsQueue.Dequeue();

        InvokeRepeating("InstantiateUnit", craftTime, craftTime);
    }

    public void intProduction()
    {
        if (unitsQueue.Count <= 0)
            return;
        /*if (unitsQueue.Dequeue().GetComponent<Unit>().prices[1]> recurces[1] || unitsQueue.Dequeue().GetComponent<Unit>().prices[2] > recurces[2])
            return;
        if (CivilizationMetrics.singleton[entity.faction].troops >= CivilizationMetrics.singleton[entity.faction].maxTroops)
            return;

        recurces[1] -= unitsQueue.Dequeue().GetComponent<Unit>().prices[1]
        recurces[2] -= unitsQueue.Dequeue().GetComponent<Unit>().prices[2]
        CivilizationMetrics.singleton[entity.faction].troops++;
        curretProduction = unitsQueue.Dequeue();*/
    }

    public void InstantiateUnit()
    {
        /*if(curretProduction.GetComponent<Unit>().time > 0)
        {
            curretProduction.GetComponent<Unit>().time--;
            return;
        }*/

        var pos = RandomInsideDonut(instanceRadius);
        Instantiate(curretProduction,
            new Vector3(pos.x + transform.position.x, 0, pos.y + transform.position.z),
            curretProduction.transform.rotation);
    }

    public void AddUnitToQueue(int unit)
    {
        unitsQueue.Enqueue(productionUnits[unit]);
    }

    public static Vector2 RandomInsideDonut(Vector2 donutRadius)
    {
        //     var p = Random.
            
  //          s.y);
        var a = Random.Range(0, 360);

    //    return new Vector2(Mathf.Sin(a * Mathf.Deg2Rad), Mathf.Cos(a * Mathf.Deg2Rad)) * p;
    return Vector2.down;
    }
}
