using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class wikiMenu : MonoBehaviour
{
    public RTSEntity info;
    public Text titel, desciption, tecnicAspects1, type, tecnicAspects2, Prices;
    public Image image;

    public void changeInfo()
    {
        image.sprite = info.Preview;
        titel.text = info.EntityName;
        desciption.text = info.Description;
        type.text = info.entityType.ToString();
        Prices.text = $"{info.Prices[0]} I  {info.Prices[1]} X  {info.Prices[2]} W  {info.Prices[3]} S  ";
        tecnicAspects1.text = "Health: " + info.MaxHealth + "";

        if (info.Prefab.GetComponent<Unit>() == null)
        {
            tecnicAspects1.text +="    Construction time: "+ info.BuildTime;
        }
        else
        {
            tecnicAspects1.text += "    Production time: " + info.BuildTime+ "    Speed: "+ info.Velocity+"    Moral: "+info.Moral+"   Expiration velocity: "+info.ExpirationVelocity;
        }

        switch (info.entityType)
        {
            case EntityType.Tower:
                tecnicAspects2.text = "Damage: " + info.Damage + "  Attack distance: " + info.AttackRange + "   Timer: " + info.AttackTimer;
                break;

            case EntityType.Fridge:
                tecnicAspects2.text = "Effect radius: " + info.EffectRadious + "    Recovery health speed: " + info.RecoverySpeed;
                break;

            case EntityType.Building:
                tecnicAspects2.text = "ProductionUnits: ";
                foreach(GameObject unit in info.ProductionUnits)
                {
                    if (unit.TryGetComponent<RTSBase>(out RTSBase unit2))
                        tecnicAspects2.text += " ,"+unit2.rtsEntity.EntityName;
                }
                break;

            case EntityType.Hero:
                tecnicAspects2.text = "";
                if (info.Damage != 0) tecnicAspects2.text += "Damage: " + info.Damage + "   Timer: " + info.AttackTimer;
                if (info.Prefab.GetComponent<AreaDamage>()) tecnicAspects2.text += "    Area damage: " + info.EffectRadious;
                if (info.RecoverySpeed != 0) tecnicAspects2.text += "Effect radius: " + info.EffectRadious + "  Recovery moral Speeds " + info.RecoverySpeed;
                if (info.DamageMoral != 0) tecnicAspects2.text += "\nDamage moral: " + info.DamageMoral + "    Efect radius: " + info.EffectRadious;
                break;

            case EntityType.PortableFridge:
                tecnicAspects2.text = "Effect radius: " + info.EffectRadious + "    Recovery health speed: " + info.RecoverySpeed;
                break;

            case EntityType.Villager:
                tecnicAspects2.text = "";
                break;

            case EntityType.Priest:
                tecnicAspects2.text = "Effect radius: " + info.EffectRadious + "    Recovery moral Speeds " + info.RecoverySpeed;
                if(info.Damage != 0) tecnicAspects2.text += "\nDamage: " + info.Damage +" Timer: " + info.AttackTimer;
                if (info.Prefab.GetComponent<AreaDamage>()) tecnicAspects2.text += "    Area damage: " + info.EffectRadious;
                break;

            case EntityType.DistanceUnit:
                tecnicAspects2.text = "Damage: " + info.Damage + "  Attack Distance: " + info.AttackRange + " Timer: " + info.AttackTimer;
                if (info.Proyectile.GetComponent<AreaDamage>()) tecnicAspects2.text += "    Area Damage: " + info.EffectRadious;
                if (info.DamageMoral != 0) tecnicAspects2.text += "\nDamage Moral: " + info.DamageMoral+ "  Efect Radius: "+ info.EffectRadious;
                break;

            case EntityType.MeleeUnit:
                tecnicAspects2.text = "Damage: " + info.Damage +"   Timer: " + info.AttackTimer;
                if (info.Prefab.GetComponent<AreaDamage>()) tecnicAspects2.text += "    Area damage: " + info.EffectRadious;
                if (info.DamageMoral != 0) tecnicAspects2.text += "\nDamage Moral: " + info.DamageMoral + "   Efect radius: " + info.EffectRadious;
                break;
        }
        
        
    }
}
