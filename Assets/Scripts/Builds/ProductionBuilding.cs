using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    GameObject[] productionUnits;
    float craftTime;

    Queue<GameObject> unitsQueue = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        productionUnits = rtsEntity.ProductionUnits;
        InvokeRepeating("InstantiateUnit", craftTime, craftTime);
    }

    public void InstantiateUnit()
    {
        if (unitsQueue.Count <= 0)
            return;
        /*if (CivilizationMetrics.singleton[entity.faction].troops >= CivilizationMetrics.singleton[entity.faction].maxTroops)
            return;

        CivilizationMetrics.singleton[entity.faction].troops++;
        var go = unitsQueue.Dequeue();
        var pos = RandomInsideDonut(instanceRadius);
        Instantiate(go,
            new Vector3(pos.x + transform.position.x, 0, pos.y + transform.position.z),
            go.transform.rotation);*/
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
