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
    [SerializeField] private GameObject unitSpawner;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private LayerMask floorMask = new LayerMask();
    private Camera mainCamera;
    private RTSPlayerv2 player;
    private Renderer buildingRendererInstance;

    public void SetUnit(Unit newUnit)
    {
        unit = newUnit;
        StartStuff();
    }

    public void StartStuff()
    {
        mainCamera = Camera.main;
        if (unit != null)
        {
            unit.prices = unit.rtsEntity.Prices;
        }

        iconImage.sprite = unit.preview;
        priceText.text = $"{unit.rtsEntity.Prices[0]} I\n" +
                         $"{unit.rtsEntity.Prices[1]} X\n" +
                         $"{unit.rtsEntity.Prices[2]} W\n" +
                         $"{unit.rtsEntity.Prices[3]} S";
    }

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();

        StartStuff();
    }

    public void SetSpawner(UnitSpawnerv3 spawner)
    {
        unitSpawner = spawner.gameObject;
    }

    private void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("ONPOINTERDOWN");

        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Debug.Log("ONPOINTERDOWnleFT");

        unit.prices = unit.rtsEntity.Prices;

        //unitSpawner.Select();
        if (unit != null)
        {
            Debug.Log("Try create unit in next line");

            if (!NetworkClient.connection.identity.isClient)
            {
                return;
            }
                player.CmdTryCreateUnit(unit.GetId(),unitSpawner.GetComponent<UnitSpawnerv3>());
             
}
        else
        {
            Debug.LogError("Unit is null ");
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
}