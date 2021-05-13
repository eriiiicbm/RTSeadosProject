using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    public bool isTesting=false;
    public GameObject cameraFocus;

    Camera myCam;
    GameObject cam;

    public float zoomSpeed = 6f;
    public Vector2 zoomLimits;

    int min = -16, max = 39;
    float margin = 0;

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float screenBrderThickness= 10f;
    [SerializeField] private Vector2 screenXLimits= Vector2.zero;
    [SerializeField] private Vector2 screenZLimits= Vector2.zero;
    private Controls controls;
    private Vector2 previousInput;


    private void Start()
    {

    }

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);
        controls = new Controls();
        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;
        controls.Enable();
        cam = GameObject.FindWithTag("MainCamera");
        if (cam != null)
        {
            myCam = cam.GetComponent<Camera>();
        }
        else
        {
            Debug.Log("Null camera component");
        }
    }
[ClientCallback]
    private void Update()
    {

        if (!hasAuthority||!Application.isFocused)
        {
            return;
        }

        UpdateCameraPosition();


        // Zoom code 
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        myCam.orthographicSize -= zoom * zoomSpeed;

        myCam.orthographicSize = Mathf.Clamp(myCam.orthographicSize,
            zoomLimits.x, zoomLimits.y);

 //       Debug.Log(zoom);

        if (zoom < 0 && margin != min && margin != min)
        {
//            Debug.Log("AUMENTO");

            cameraFocus.gameObject.transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);

            if (margin > -16)
            {
                margin--;
            }
        }

        if (zoom > 0 && margin != max && margin != max)
        {
  //          Debug.Log("DISMINUCION");

            cameraFocus.gameObject.transform.localScale -= new Vector3(0.25f, 0.25f, 0.25f);

            if (margin < 39)
            {
                margin++;
            }

        }
 //       Debug.Log(margin);

    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = playerCameraTransform.position;
        if (previousInput == Vector2.zero)
        {
            Vector3 cursorMovement = Vector3.zero;
            Vector2 cursorPosition = Mouse.current.position.ReadValue();
            if (cursorPosition.y >= Screen.height - screenBrderThickness&&!isTesting)
            {
                cursorMovement.z += 1;
            }
            else if (cursorPosition.y <= screenBrderThickness&&!isTesting)
            {
                cursorMovement.z -= 1;
            }

            if (cursorPosition.x >= Screen.width - screenBrderThickness&&!isTesting)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBrderThickness&&!isTesting)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * speed * Time.deltaTime;
        }
        else
        {
            pos += new Vector3(previousInput.x, 0f, previousInput.y) * speed * Time.deltaTime;

        }

        pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenXLimits.y);
        playerCameraTransform.position = pos;
    } 


    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}
