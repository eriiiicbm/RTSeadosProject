using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UnitSelectionHandlerv2 : MonoBehaviour

{
    private Controls controls;
    public bool isOneClick;
    [SerializeField] private RectTransform unitSelectionArea = null;

    [SerializeField] private LayerMask layerMask = new LayerMask();
    public GameObject buildingsDisplay;
    private Vector2 startPosition;

    private RTSPlayerv2 player;
    private Camera mainCamera;
    int villagersNumber = 0;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();
    [SerializeField] public List<Building> SelectedBuildings { get; } = new List<Building>();

    private void Start()
    {
        controls = new Controls();
        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;
        controls.Enable();
        mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();

    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    [Client]
    public void AddBuildingToSelectedBuildings(Building b)
    {
        SelectedBuildings.Add(b);
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.IsPointerOverGameObject() &&
                EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null)
            {
                return;
            }
        }


        if (NetworkClient.connection == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {



            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    [Client]
    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }

            foreach (var selectedBuilding in SelectedBuildings)
            {
                selectedBuilding.Deselect();
            }

            SelectedUnits.Clear();

            ActionsMenu._instance.TurnDestroyMenu(false);
            SelectedBuildings.Clear();
        }

        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }

    [Client]
    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition +
                                             new Vector2(areaWidth / 2, areaHeight / 2);
    }

    [Client]
    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);
        buildingsDisplay.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude == 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                return;
            }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit))
            {
                goto build;
            }

            if (!unit.hasAuthority)
            {
                goto build;
            }

            SelectedUnits.Add(unit);
            CheckIfVilager();
            Debug.Log("case 1");
            build:
            if (!hit.collider.TryGetComponent<Building>(out Building building))
            {
                return;
            }


            if (building == null) return;
            if (!building.hasAuthority)
            {
                return;
            }

            SelectedBuildings.Add(building);

            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit))
            {
                continue;
            }

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if (screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }

        foreach (Building building in player.GetMyBuildings())
        {
            if (SelectedBuildings.Contains(building))
            {
                continue;
            }

            Vector3 screenPosition = mainCamera.WorldToScreenPoint(building.transform.position);

            if (screenPosition.x > min.x &&
                screenPosition.x < max.x &&
                screenPosition.y > min.y &&
                screenPosition.y < max.y)
            {
                SelectedBuildings.Add(building);
                building.Select();
            }
        }

        CheckIfVilager();

        Debug.Log("case 2");
    }

    public bool CheckIfVilager()
    {
        buildingsDisplay.SetActive(false);

        villagersNumber = 0;
        if (SelectedBuildings.Count > 0) return false;
        foreach (Unit selectedUnit in SelectedUnits)
        {
            selectedUnit.Select();
            if (selectedUnit.GetComponent<Villager>() != null)
            {
                villagersNumber++;
            }
        }

        if (SelectedUnits.Count == 0)
        {
            return false;
        }

        if (villagersNumber != SelectedUnits.Count) return false;
        Debug.Log("All the selected units are villagers");
        buildingsDisplay.SetActive(true);
        return true;
    }

    public void ClearVillager()
    {
        if (CheckIfVilager())
        {
            foreach (Unit selectedUnit in SelectedUnits)
            {
                selectedUnit.Select();

                Villager villager = selectedUnit.GetComponent<Villager>();

                if (villager != null)
                {
                    villager.building = null;
                    villager.resource = null;
                }
            }
        }
    }

    [ContextMenu("DestroySelected")]
    public void CmdDestroySelected()
    {
        Debug.Log("destroying");
        foreach (var unit in SelectedUnits)
        {
            Debug.Log($"Destroying {unit.name}");
            unit.CmdDestroy();


        }
    }

    [ContextMenu("SelectAllUnits")]
    [Client]
    public void SelectAllUnits()
    {
        foreach (Unit unit in player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit))
            {
                continue;
            }

            SelectedUnits.Add(unit);
            unit.Select();
        }
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
        if (SelectedUnits.Count == 0)
        {
            ActionsMenu._instance.TurnDestroyMenu(false);
        }
    }

    [Client]
    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        isOneClick = ctx.ReadValueAsButton();
    }

    public void ToggleAutomaticBehaviour(bool newState)
    {
        foreach (var unit in SelectedUnits)
        {
            UnitCombat unitCombat = unit.GetComponent<UnitCombat>();
            if (unitCombat == null)
            {
                continue;
            }

            unitCombat.automaticAttack = newState;


        }
    }

    public bool CheckIfAllSelectedUnitsHaveTheAutomaticBehaviour()
    {

        foreach (var unit in SelectedUnits)
        {
            if (unit.GetComponent<UnitCombat>()?.automaticAttack == false)
            {
                return false;
            }
        }

        return true;
    }
}

