using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WikiMenu : MonoBehaviour
{
    public RTSEntity info;
    public Text titel, desciption, tecnicAspects1, type, tecnicAspects2, Prices;
    public Image image;
    public TextStrings statsNames;

    public void ChangeInfo()
    {
        var statsNames = GameManager.getStrings(this.statsNames);
        image.sprite = info.Preview;
        titel.text =GameManager.getStrings(info.NameText)[0];
        desciption.text = GameManager.getStrings(info.DescriptionText)[0];
        type.text = info.entityType.ToString();
        Prices.text = $"{info.Prices[0]} I  {info.Prices[1]} X  {info.Prices[2]} W  {info.Prices[3]} S  ";
        tecnicAspects1.text = statsNames[0] + info.MaxHealth + "";

        if (info.Prefab.GetComponent<Unit>() == null)
        {
            tecnicAspects1.text +=  statsNames[1]+ info.BuildTime;
        }
        else
        {
            tecnicAspects1.text += statsNames[2] + info.BuildTime+ statsNames[2]+ info.Velocity+statsNames[3] +info.Moral+  statsNames[4]+info.ExpirationVelocity;
        }

        switch (info.entityType)
        {
            case EntityType.Tower:
                tecnicAspects2.text = statsNames[6] + info.Damage +statsNames[7] + info.AttackRange +statsNames[8] + info.AttackTimer;
                break;

            case EntityType.Fridge:
                tecnicAspects2.text = statsNames[9] + info.EffectRadious + statsNames[10] + info.RecoverySpeed;
                break;

            case EntityType.Building:
                tecnicAspects2.text = statsNames[11];
                foreach(GameObject unit in info.ProductionUnits)
                {
                    //if (unit.TryGetComponent<RTSBase>(out RTSBase unit2))
                      //  tecnicAspects2.text += " ,"+unit2.rtsEntity.EntityName;
                }
                break;

            case EntityType.Hero:
                tecnicAspects2.text = "";
                if (info.Damage != 0) tecnicAspects2.text += statsNames[6] + info.Damage + statsNames[8] + info.AttackTimer;
                if (info.Prefab.GetComponent<AreaDamage>()) tecnicAspects2.text += statsNames[12] + info.EffectRadious;
                if (info.RecoverySpeed != 0) tecnicAspects2.text += statsNames[9] + info.EffectRadious + statsNames[13]  + info.RecoverySpeed;
                if (info.DamageMoral != 0) tecnicAspects2.text += "\n" + statsNames[14] + " "+ info.DamageMoral + statsNames[9] + info.EffectRadious;
                break;

            case EntityType.PortableFridge:
                tecnicAspects2.text = statsNames[9] + info.EffectRadious + statsNames[15] + info.RecoverySpeed;
                break;

            case EntityType.Villager:
                tecnicAspects2.text = "";
                break;

            case EntityType.Priest:
                tecnicAspects2.text = statsNames[9] + info.EffectRadious + statsNames[13] + info.RecoverySpeed;
                if(info.Damage != 0) tecnicAspects2.text += "\n" + statsNames[6] +  info.Damage + statsNames[8] + info.AttackTimer;
                if (info.Prefab.GetComponent<AreaDamage>()) tecnicAspects2.text += statsNames[12] + info.EffectRadious;
                break;

            case EntityType.DistanceUnit:
                tecnicAspects2.text =statsNames[6]+ info.Damage +statsNames[7]+ info.AttackRange + statsNames[8] + info.AttackTimer;
                if (info.Proyectile.GetComponent<AreaDamage>()) tecnicAspects2.text += statsNames[12] + info.EffectRadious;
                if (info.DamageMoral != 0) tecnicAspects2.text += "\n" + statsNames[14] +  + info.DamageMoral+  statsNames[9]+ info.EffectRadious;
                break;

            case EntityType.MeleeUnit:
                tecnicAspects2.text = statsNames[6]+ info.Damage +statsNames[8] + info.AttackTimer;
                if (info.Prefab.GetComponent<AreaDamage>()) tecnicAspects2.text += statsNames[12] + info.EffectRadious;
                if (info.DamageMoral != 0) tecnicAspects2.text += "\n" + statsNames[14] + info.DamageMoral + statsNames[9] + info.EffectRadious;
                break;
        }
        
        
    }
}
