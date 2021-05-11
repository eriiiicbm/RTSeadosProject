using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Utilities : MonoBehaviour
{
    public static Action<int> doorEvent;
    public static Action playerCrouching;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 public   static  IEnumerator PauseWhile(System.Func<bool> condition)
    {
        if (condition == null)
        {
            yield break;
        }

        while (!condition())
        {
            yield return null;
        }
    }
}
