using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Transform followTransform;
    public Transform cameraTransform;

    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;
    public Transform groundTarget;

    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    private void Awake()
   {
       groundTarget.position = Utils.MiddleOfScreenPointToWorld();
   }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
        zoomAmount = new Vector3(0, -12, 12);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameIsPaused) return;
        
        if (followTransform != null)
        {
            transform.position = followTransform.position;
        }
        else
        {
            HandleMovementInput();
            Vector3 middle = Utils.MiddleOfScreenPointToWorld();
            groundTarget.position = middle;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void HandleMovementInput()
    {
        // WASD movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += (transform.forward * movementSpeed * (1 +(newZoom.y / 100)));
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += (transform.forward * -movementSpeed * (1 +(newZoom.y / 100)));
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += (transform.right * movementSpeed * (1 +(newZoom.y / 100)));
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += (transform.right * -movementSpeed * (1 +(newZoom.y / 100)));
        }

        // camera rotation
        if (Input.GetKey(KeyCode.Q))
        {
            newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
        }
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
        if (Input.GetKey(KeyCode.Space) ) //FIXME make this smooth
        {
            transform.rotation = Quaternion.identity;
            newRotation = Quaternion.identity;
        }

        // Zoom in/out
        //Vector3 rigOrigin = new Vector3(0, 1, 0);
        if (Input.GetAxis("Mouse ScrollWheel") > 0f )
        {
            // zoom in boundary
            if (newZoom.y > groundTarget.position.y-10) 
            {
                newZoom += zoomAmount;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f )
        {
            // zoom out boundary
            if (newZoom.y < groundTarget.position.y+200) 
            {
                newZoom -= zoomAmount;
            }
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
