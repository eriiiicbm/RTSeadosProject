using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : Building
{
    GameObject[] productionUnits;

    // Start is called before the first frame update
    void Start()
    {
        productionUnits = rtsEntity.ProductionUnits;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
