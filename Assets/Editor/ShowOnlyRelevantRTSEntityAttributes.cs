using System.Reflection;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

[CustomEditor(typeof(RTSEntity))]
public class ShowIfAttributeDrawer   
    : Editor
{
    SerializedProperty boolProperty;
    SerializedProperty floatProperty;
    SerializedProperty intProperty;
    SerializedProperty enumProperty;


    void OnEnable()
    {
   //     boolProperty = serializedObject.FindProperty("overdriveEnhanced");
     //   floatProperty = serializedObject.FindProperty("overdriveDamage");
        enumProperty = serializedObject.FindProperty("entityType");
 
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();
  //      EditorGUILayout.PropertyField(enumProperty);
        /*   switch ((EntityType) enumProperty.enumValueIndex)
           {
               case EntityType.Unit:
              //     EditorGUILayout.PropertyField(sectionA);
                   break;
               case EntityType.MeleeUnit:
                //   EditorGUILayout.PropertyField(sectionB);
                   break;
               case EntityType.CombatUnit:
                  // EditorGUILayout.PropertyField(sectionB);
                   break;
               case EntityType.DistanceUnit:
                  // EditorGUILayout.PropertyField(sectionB);
                   break;
               case EntityType.Fridge:
                   //EditorGUILayout.PropertyField(sectionB);
                   break;
           }
          */
        //serializedObject.ApplyModifiedProperties();

   //     enumProperty.enumValueIndex = EditorGUILayout.Toggle(enumProperty.displayName,enumProperty. );
      //  boolProperty.boolValue = EditorGUILayout.Toggle(boolProperty.displayName, boolProperty.boolValue);

  /*      if (boolProperty.boolValue)
        {
            OnBoolPropertyTrue();
        }
        */
        RTSEntity  myScript = target as RTSEntity;
        List<string> excludedProperties = new List<string>();
        ExcludeProperties(myScript, excludedProperties);
        Debug.Log(
            "EndInspectorGUi");

        //intProperty.intValue = EditorGUILayout.IntField(intProperty.displayName, intProperty.intValue);

    }
    void ExcludeProperties(RTSEntity rTSEntity, List<string> excludedProperties)
        {

        excludedProperties.Add("craftUnCompletedGO");
        excludedProperties.Add("craftCompletedGO");
        excludedProperties.Add("buildRenderer");
        excludedProperties.Add("currentProduction");
        excludedProperties.Add("damageMoral");
        excludedProperties.Add("unitsQueue");

        switch ((EntityType)rTSEntity.entityType)
        {
            case EntityType.UnitCombat:
                     excludedProperties.Add("damage");
                 excludedProperties.Add("attackRange");
                excludedProperties.Add("craftRadious");
                excludedProperties.Add("craftCompletedGO");
                excludedProperties.Add("craftUnCompletedGO");
                excludedProperties.Add("buildRenderer");
                excludedProperties.Add("buildTime");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("productionUnits");
                excludedProperties.Add("craftUnCompletedGO");
                 excludedProperties.Add("effectRadious");
                excludedProperties.Add("recoverySpeed");
                excludedProperties.Add("proyectile");
               

                break;
            
            case EntityType.Fridge:
                excludedProperties.Add("expirationVelocity");
                excludedProperties.Add("expirationVelocity");
                excludedProperties.Add("moral");
                excludedProperties.Add("velocity");
                excludedProperties.Add("damage");
                excludedProperties.Add("attackRange");
                excludedProperties.Add("attackTimer");
                excludedProperties.Add("proyectile");
                excludedProperties.Add("curreentTarget");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("productionUnits");


                break;
            case EntityType.Building:
                    excludedProperties.Add("expirationVelocity");
                    excludedProperties.Add("moral");
                    excludedProperties.Add("velocity");
                    excludedProperties.Add("damage");
                    excludedProperties.Add("attackRange");
                    excludedProperties.Add("attackTimer");
                excludedProperties.Add("canCraft");
                excludedProperties.Remove("unitsQueue");





                break; 
 case EntityType.Priest:
                excludedProperties.Remove("effectRadious");
                excludedProperties.Remove("recoverySpeed");
                excludedProperties.Add("proyectile");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("craftRadious");
                excludedProperties.Add("buildTime");
                excludedProperties.Add("productionUnits");
                excludedProperties.Remove("damageMoral");

                break;
            case EntityType.Villager:
                excludedProperties.Add("effectRadious");
                excludedProperties.Add("recoverySpeed");
                excludedProperties.Add("damage");
               //todo cambiar nombre a attackrange en este caso
                excludedProperties.Add("productionUnits");
                excludedProperties.Add("proyectile");
                excludedProperties.Add("buildTime");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("craftRadious");


                //todo cambiar nombre a  Attacktimer
                break;
            case EntityType.DistanceUnit:
                excludedProperties.Remove("proyectile");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("craftRadious");
                excludedProperties.Add("productionUnits");
                excludedProperties.Add("recoverySpeed");
                excludedProperties.Add("buildTime");


                break;
            case EntityType.Tower:
                excludedProperties.Remove("proyectile");
                excludedProperties.Add("moral");
                excludedProperties.Add("productionUnits");
                excludedProperties.Add("canCraft");

                excludedProperties.Add("effectRadious");
                excludedProperties.Add("recoverySpeed");
                excludedProperties.Add("currentTarget");


                break;
            case EntityType.PortableFridge:

                excludedProperties.Add("proyectile");
                excludedProperties.Add("damage");
                excludedProperties.Add("attackTimer");
                excludedProperties.Add("craftRadious");
                excludedProperties.Add("productionUnits");
                excludedProperties.Add("buildTime");
                excludedProperties.Add("attackRange");
                excludedProperties.Add("attackTimer");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("effectRadious");
                excludedProperties.Add("recoverySpeed");

                break;
            case EntityType.Hero:
                excludedProperties.Remove("damageMoral");
                excludedProperties.Add("canCraft");
                excludedProperties.Add("proyectile");
                excludedProperties.Add("productionUnits");
                excludedProperties.Add("craftRadious");

                break;
        }
        DrawPropertiesExcluding(serializedObject, excludedProperties.ToArray());
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();

        Debug.Log("Applied exlcuded properties");


    }

    void OnBoolPropertyTrue()
    {
        floatProperty.floatValue = EditorGUILayout.FloatField(floatProperty.displayName, floatProperty.floatValue);
        EditorGUILayout.LabelField("I AM BELOW THE BOOL PROPERTY NOW");
    }
}
  
