using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class UnitSelectionHandler : MonoBehaviour

{
    private Camera mainCamera;
    [SerializeField] private LayerMask layerMask = new LayerMask();
    [SerializeField] private RectTransform unitSelectionArea;
    private RTSPlayer player=null;
    private Vector2 startPosiion;
    [SerializeField] public List<Unit> SelectedUnits { get; } = new List<Unit>();

    // Start is called before the first frame update
    void Start()
    {
        mainCamera=Camera.main;
    //    player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( NetworkClient.connection!=null)
        {
            
        if (player==null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        }
        }
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {StartSelectionArea();
        
        }else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        float areaWidth = mousePosition.x - startPosiion.x;
        float areaHeight = mousePosition.y - startPosiion.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosiion + new Vector2(areaWidth / 2, areaHeight / 2);

    }
 private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            foreach (var selectedUnit in SelectedUnits)
            {
                selectedUnit.Deselect();
            }
            SelectedUnits.Clear();
        }
     
        unitSelectionArea.gameObject.SetActive(true);
        startPosiion = Mouse.current.position.ReadValue();
        UpdateSelectionArea();
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);
        if (unitSelectionArea.sizeDelta.magnitude==0)
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
            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);
        foreach (var unit in player.GetMyUnits())
        {
            if (SelectedUnits.Contains(unit))
            {
                continue;
            }
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (screenPosition.x>min.x&& screenPosition.x<max.x&& screenPosition.y>min.y&& screenPosition.y< max.y)
            {
             SelectedUnits.Add(unit);
             unit.Select();
            }
        }

    }
}
