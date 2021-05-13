using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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

    public List<Unit > SelectedUnits { get; } = new List<Unit >();

    private void Start()
    {
        controls = new Controls();
        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;
        controls.Enable();
        mainCamera = Camera.main;

        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }
   
    private void Update()
    {
       

        if (player == null)
        {
                player = NetworkClient.connection.identity.GetComponent<RTSPlayerv2>();
        }
        if (isOneClick) {
            StartSelectionArea();
            ClearSelectionArea();
        }
        if (Mouse.current.leftButton.wasPressedThisFrame&&Mouse.current.leftButton.wasReleasedThisFrame) {
            StartSelectionArea();
            ClearSelectionArea();
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

            SelectedUnits.Clear();
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

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit >(out Unit  unit)) { return; }

            if (!unit.hasAuthority) { return; }

            SelectedUnits.Add(unit);
            foreach (Unit selectedUnit in SelectedUnits)
            {

                selectedUnit.Select();
                if (selectedUnit.GetComponent<Villager>() != null)
                {

                    villagersNumber++;
                }
            }



            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit)) { continue; }

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
          villagersNumber = 0;
        foreach (Unit selectedUnit in SelectedUnits)
        {     
 
            selectedUnit.Select();
            if (selectedUnit.GetComponent<Villager>()!=null)
            {
 
                villagersNumber++;
            }
        }

        if (SelectedUnits.Count==0)
        {
            return;
        }

        if (villagersNumber != SelectedUnits.Count) return;
        Debug.Log("All the selected units are villagers");
        buildingsDisplay.SetActive(true);
        return;

    }

    private void AuthorityHandleUnitDespawned(Unit  unit)
    {
        SelectedUnits.Remove(unit);
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
}
