using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class DayNightSystem : MonoBehaviour
{ 
    SpriteRenderer sprite;
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField, Range(0, 24)] private float TimeOfDay;
   [SerializeField] private float speed=50;
   public DayOrNight dayOrNight;
   public Action OnDay;
   public Action OnNight;
    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
             TimeOfDay += Time.deltaTime/speed;
            TimeOfDay %= 24; 
            UpdateLighting(TimeOfDay / 24f);
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }

        if (TimeOfDay!=20f)
        {
            return;
        }

        dayOrNight = DayOrNight.Day;
        OnDay?.Invoke();
    }


    private void UpdateLighting(float timePercent)
    {
         RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

         if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

    }



  
    // Start is called before the first frame update
    void Start()
    { 
        sprite = GetComponentInParent<SpriteRenderer>();
    }

 
}

public  enum DayOrNight
{
    Day,Night
    
    
    
}
 