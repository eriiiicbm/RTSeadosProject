using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitBuildingButtonv2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
      [SerializeField] private Unit unit;
     [SerializeField] private Image iconImage;
     [SerializeField] private UnitSpawnerv3 unitSpawner;
     [SerializeField] private TMP_Text priceText;
     [SerializeField] private LayerMask floorMask = new LayerMask();
     private Camera mainCamera;
     private RTSPlayerv2 player;
    UnitSpawnerv3 unitSpawnerv3;
     private Renderer buildingRendererInstance;
    public void SetUnit(Unit newUnit) {
        unit = newUnit;

    } 
     private void Start()
     {
         mainCamera = Camera.main;
         iconImage.sprite = unit.preview;
         priceText.text = unit.prices.ToString();
       }

     public void SetSpawner(UnitSpawnerv3 spawner)
     {
         unitSpawnerv3 = spawner;
     }
     private void Update()
     {
         if (player == null)
         {
             player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
         }


     }

     public void OnPointerDown(PointerEventData eventData)
     {
         if (eventData.button != PointerEventData.InputButton.Left)
         {
             return;
         }

         if (!player.CheckIfUserHasResources(unit.prices))
         {
            unitSpawnerv3.AddUnitToTheQueue(unit);

        }

       
     }

     public void OnPointerUp(PointerEventData eventData)
     {
         

         Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
         if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
         {
           //  Debug.Log("Try place " + unit.GetId() + " in " + hit.point);
            // player.CmdTryPlaceBuilding(unit.GetId(), hit.point);
         }

         //Destroy(buildingPreviewInstance);
     }

     private void UpdateBuildingPreview()
     {
       /*  Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
         if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
         {
             return;
         }

         buildingPreviewInstance.transform.position = new Vector3 (hit.point.x,buildingPreviewInstance.transform.position.y,hit.point.z);
         if (!buildingPreviewInstance.activeSelf)
         {
             buildingPreviewInstance.SetActive(true);
         }

         Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;
         Debug.Log($"color{color.ToString()}]");
         buildingRendererInstance.material.SetColor("_Color", color);
     */} 
    
}