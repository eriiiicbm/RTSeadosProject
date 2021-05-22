using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayv2 : MonoBehaviour
{
  [SerializeField] private RTSBase health;
  [SerializeField] private GameObject healtBarParent;
  [SerializeField] private Image healthBarImage;


  private void Awake()
  {
    health = GetComponent<RTSBase>();
    health.ClientOnHealthUpdated += HandleHealthUpdated;
    healtBarParent.SetActive(true);

  }

  private void OnDestroy()
  {
    health.ClientOnHealthUpdated -= HandleHealthUpdated;}

  private void OnMouseEnter()
  {
//healtBarParent.SetActive(true);
}

  private void OnMouseExit()
  {
  //  healtBarParent.SetActive(false);
  }

  private void HandleHealthUpdated(float currentHealth, float maxHealth)
  {
     healthBarImage.fillAmount = (float)currentHealth / 
                                maxHealth;
  }
}
