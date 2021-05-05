using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour {

    RaycastHit hit;
    List<UnitController> selectedUnits = new List<UnitController>();
    bool isDragging = false;
    Vector3 mousePositon;
    GameObject TargetObj;

    public GameObject Target;
    private float raycastLength = 500;
    string name = "Target Instantiated";

    private void OnGUI()
    {
        if(isDragging)
        {
            var rect = ScreenHelper.GetScreenRect(mousePositon, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, new Color(0.3f, 0.8f, 0.3f, 0.1f));
            ScreenHelper.DrawScreenRectBorder(rect, 0.6f, Color.green);
        }
        
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update () {
		
        //Detect if mouse is down
        if(Input.GetMouseButtonDown(0))
        {
            mousePositon = Input.mousePosition;
            //Create a ray from the camera to our space
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Shoot that ray and get the hit data
            if(Physics.Raycast(camRay, out hit))
            {
                //Do something with that data 
                //Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("PlayerUnit"))
                {
                    SelectUnit(hit.transform.GetComponent<UnitController>(), Input.GetKey(KeyCode.LeftShift));
                }
                else
                {
                    isDragging = true;
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                DeselectUnits();
                foreach (var selectableObject in FindObjectsOfType<PlayerUnitController>())
                {
                    if (IsWithinSelectionBounds(selectableObject.transform))
                    {
                        SelectUnit(selectableObject.gameObject.GetComponent<UnitController>(), true);
                    }
                }

                isDragging = false;
            }
            
        }

        if(Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Shoot that ray and get the hit data
            if (Physics.Raycast(camRay, out hit))
            {
                //Do something with that data 
                //Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("Ground"))
                {

                    GameObject go = GameObject.Find(name);

                    if (TargetObj)
                    {
                        Debug.Log("exists");
                        Destroy(go.gameObject);
                        Debug.Log(name + "has been destroyed.");

                    }
                foreach (var selectableObj in selectedUnits)
                    {
                        selectableObj.MoveUnit(hit.point);

                    }

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, raycastLength))
                    {
                        if (hit.collider.name == "Terrain")
                        {
                            TargetObj = Instantiate(Target, hit.point, Quaternion.identity) as GameObject;
                            TargetObj.name = "Target Instantiated";

                        }
                    }
                    Debug.DrawRay(ray.origin, ray.direction * raycastLength, Color.yellow);

                }
                else if (hit.transform.CompareTag("EnemyUnit"))
                {
                    foreach (var selectableObj in selectedUnits)
                    {
                        selectableObj.SetNewTarget(hit.transform);
                    }
                }
            }
        }

    }

    private void SelectUnit(UnitController unit, bool isMultiSelect = false)
    {
        if(!isMultiSelect)
        {
            DeselectUnits();
        }
        selectedUnits.Add(unit);
        unit.SetSelected(true);
    }

    private void DeselectUnits()
    {
        for(int i = 0; i < selectedUnits.Count; i++)
        {
            // selectedUnits[i].Find("Highlight").gameObject.SetActive(false);
            selectedUnits[i].SetSelected(false);
        }
        selectedUnits.Clear();
    }

    private bool IsWithinSelectionBounds(Transform transform)
    {
        if(!isDragging)
        {
            return false;
        }

        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera, mousePositon, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }
}
