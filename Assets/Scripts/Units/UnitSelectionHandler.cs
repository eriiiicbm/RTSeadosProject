using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UnitSelectionHandler : MonoBehaviour

{
    private Camera mainCamera;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    [SerializeField] public List<Unit> SelectedUnits { get; } = new List<Unit>();

    // Start is called before the first frame update
    void Start()
    {
        mainCamera=Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            SelectedUnits.Clear();
        }else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray,out  RaycastHit hit,Mathf.Infinity,layerMask))
        {
            return;
        }

        if (!hit.collider.TryGetComponent<Unit>(out  Unit unit))
        {
            return;
        }

        if (!unit.hasAuthority)
        {
            return;
        }
        SelectedUnits.Add(unit);

        foreach (var selectedUnit in SelectedUnits)
        {
            selectedUnit.Select();
        }
    }
}
