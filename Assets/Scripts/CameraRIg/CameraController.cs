using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;

    public float movementSpeed;
    public float movementTime;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Vector3 newZoom;

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
    }

    void HandleMovementInput()
    {
        // WASD movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed * (newZoom.y / 8));
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed * (newZoom.y / 8));
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed * (newZoom.y / 8));
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed * (newZoom.y / 8));
        }

        // Zoom in/out
        Vector3 rigOrigin = new Vector3(0, 1, 0);
        if (Input.GetKey(KeyCode.Q))
        {
            // zoom in boundary
            if (newZoom.y > 10 && newZoom.z < 10) 
            {
                newZoom += zoomAmount;
            }
        }
        if (Input.GetKey(KeyCode.E))
        {
            // zoom out boundary
            if (newZoom.y < rigOrigin.y+100 && newZoom.z < rigOrigin.z+100) 
            {
                newZoom -= zoomAmount;
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}