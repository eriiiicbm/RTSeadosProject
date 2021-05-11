using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Movement
    public float borderMoveSpeed = 1.2f;
    public float screenOffset = .005f;
    // ZOOM
    public float zoomSpeed = 6f;
    public Vector2 zoomLimits;

    public GameObject cameraFocus;
    Camera myCam;

    int min = -16, max = 39;
    float margin = 0; 
    private void Start()
    {
        myCam = GetComponent<Camera>();
    }

    void Update()
    {
        // Zoom code 
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        myCam.orthographicSize -= zoom * zoomSpeed;

        myCam.orthographicSize = Mathf.Clamp(myCam.orthographicSize,
            zoomLimits.x, zoomLimits.y);
            

        if(zoom < 0 && margin != min && margin != min) { 
            cameraFocus.gameObject.transform.localScale += new Vector3(0.25f, 0.25f, 0.25f);

            if (margin > -16)
            {
                margin--;
            }
        }

        if(zoom > 0 && margin != max && margin != max)
        {
            cameraFocus.gameObject.transform.localScale -= new Vector3(0.25f, 0.25f, 0.25f);

            if(margin < 39) {
                margin++;
            }


        }

        Debug.Log(margin);

        // Camera movement per border
        Vector3 Speed = new Vector3();

        if (Input.mousePosition.x < Screen.width * screenOffset)
            Speed.x -= borderMoveSpeed;
        else if (Input.mousePosition.x > Screen.width - (Screen.width * screenOffset))
            Speed.x += borderMoveSpeed;

        if (Input.mousePosition.y < Screen.height * screenOffset)
            Speed.z -= borderMoveSpeed;
        else if (Input.mousePosition.y > Screen.height - (Screen.height * screenOffset))
            Speed.z += borderMoveSpeed;

        transform.position += Speed * Time.deltaTime;
    }
}