using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiverv2 : MonoBehaviour
{
 
    [SerializeField] private UnitSelectionHandlerv2 unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;


    GameObject TargetObj;

    public GameObject Target;
    private float raycastLength = 500;
    string name = "Target Instantiated";

    private void Start()
    {
        mainCamera = Camera.main;

        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {


        if (!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }



        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            //if the unit is yours move if not target it
            //todo state machine
            if (target.hasAuthority)
            {

                TryMove(hit.point);
                return;
            }

            TryTarget(target);
            return;
        }
        if (hit.transform.CompareTag("Floor"))
        {

            GameObject go = GameObject.Find(name);

            if (TargetObj)
            {
                Debug.Log("exists");
                Destroy(go.gameObject);
                Debug.Log(name + "has been destroyed.");

            }
            TryMove(hit.point);
            Debug.Log("TRYMOVE SINSINSIN RETURN");
            ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit, raycastLength))
            {
                if (hit.collider.name == "Floor")
                {
                    TargetObj = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
                    TargetObj.name = "Target Instantiated";

                }
            }
            Debug.DrawRay(ray.origin, ray.direction * raycastLength, Color.yellow);
        }
    }

    private void TryMove(Vector3 point)
    {


        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.CmdMove(point);
        }
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }

    private void ClientHandleGameOver(string winnerName)
    {
        enabled = false;
    }

}