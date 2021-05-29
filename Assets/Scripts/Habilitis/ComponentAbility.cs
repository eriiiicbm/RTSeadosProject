using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ComponentAbility
{
    public void SetParameter(float parameter);
    // active the hability
    void active(RTSBase unit, float damage);
}
