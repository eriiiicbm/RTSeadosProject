using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class MoralDisplayv2 : MonoBehaviour
{
  [SerializeField] private Unit moral;
  [SerializeField] private GameObject moralBarParent;
  [SerializeField] private Image moralBarImage;


  private void Awake()
  {
    if (moral==null)
    {
     
      moral = GetComponent<Unit>();

    }  
    moral.ClientOnMoralUpdated += HandleMoralUpdated;
    moralBarParent.SetActive(true);

  }

  private void OnDestroy()
  {
    moral.ClientOnMoralUpdated -= HandleMoralUpdated;
  }

  private void OnMouseEnter()
  {
  //  moralBarParent.SetActive(true);
  }

  private void OnMouseExit()
  {
   // moralBarParent.SetActive(false);
  }

  private void HandleMoralUpdated(float currentMoral, float maxMoral)
  {
//   Debug.Log($"{currentMoral}  of  {maxMoral}  the bar is updated to  {currentMoral/maxMoral} of unit {gameObject.name} of player  {NetworkClient.connection.connectionId}");

    moralBarImage.fillAmount = (float)currentMoral / maxMoral;
  }
}
