using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSFoodManager : MonoBehaviour
{
    public static void UnitTakeDamage(UnitCombat attackingController, UnitCombat attackedController)
    {
        var damage = attackedController.damage;

   //     attackedController.TakeDamage(attackedController, damage);
    }
}
