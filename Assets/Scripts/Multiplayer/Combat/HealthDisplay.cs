using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
  [SerializeField] private Health health;
  [SerializeField] private GameObject healtBarParent;
  [SerializeField] private Image healthBarImage;


  private void Awake()
  {
    health.ClientOnHealthUpdated += HandleHealthUpdated;
  }

  private void OnDestroy()
  {
    health.ClientOnHealthUpdated -= HandleHealthUpdated;}

  private void OnMouseEnter()
  {
healtBarParent.SetActive(true);  }

  private void OnMouseExit()
  {
    healtBarParent.SetActive(false);
  }

  private void HandleHealthUpdated(int currentHealth, int maxHealth)
  {
    healthBarImage.fillAmount = (float)currentHealth / maxHealth;
  }
}
