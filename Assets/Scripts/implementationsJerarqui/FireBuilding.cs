using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FireBuilding 
{
    void detectEnemy(float atackRange);

    void fire(Transform currentTarget);
}
