using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButtonv2 : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private LayerMask floorMask = new LayerMask();
    private BoxCollider buildingCollider;
    private Camera mainCamera;
    private RTSPlayerv2 player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private void Start()
    {
        mainCamera = Camera.main;
        iconImage.sprite = building.GetIcon();
        building.SetPrice(building.rtsEntity.Prices);
        priceText.text = $"{building.rtsEntity.Prices[0]} I\n" +
            $"{building.rtsEntity.Prices[1]} X\n" +
            $"{building.rtsEntity.Prices[2]} W\n" +
            $"{building.rtsEntity.Prices[3]} S";
        buildingCollider = building.GetComponent<BoxCollider>();
        player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();

    }

    private void Update()
    {
     

        if (buildingPreviewInstance == null)
        {
            return;
        }

        UpdateBuildingPreview();
    }


   

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (!player.CheckIfUserHasResources(building.GetPrice()))
        {
            return;
        }

        buildingPreviewInstance = Instantiate(building.getBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();
        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            Debug.Log("Try place " + building.GetId() + " in " + hit.point);
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            return;
        }

        buildingPreviewInstance.transform.position = new Vector3 (hit.point.x,buildingPreviewInstance.transform.position.y ,hit.point.z);
        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }

        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;
        Debug.Log($"color{color.ToString()}]");
        buildingRendererInstance.material.SetColor("_Color", color);
    }
}