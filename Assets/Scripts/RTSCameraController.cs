using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    public static RTSCameraController instance;

    public Transform cameraTransform;
    public Transform followTransform;

    public bool canMove = true;
    public float fastSpeed;
    public float normalSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public float minZoom;
    public float maxZoom;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //Default transform
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(followTransform != null)
        {
            newPosition = followTransform.position;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
            RotateCamera();
            ZoomCamera();
        }
        else
        {
            PanCamera();
            RotateCamera();
            ZoomCamera();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            followTransform = null;
        }
    }

    void PanCamera()
    {
        if (canMove)
        {
            //Dragging world with mouse click, panning
            if (Input.GetMouseButtonDown(0))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    dragStartPosition = ray.GetPoint(entry);
                }
            }

            //Dragging world with mouse click, panning
            if (Input.GetMouseButton(0))
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    dragCurrentPosition = ray.GetPoint(entry);

                    newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                }
            }

            //Fast movement
            if (Input.GetKey(KeyCode.LeftShift))
            {
                movementSpeed = fastSpeed;
            }
            else
            {
                movementSpeed = normalSpeed;
            }

            //WASD / Arrow Key Movement
            //Forward
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                newPosition += (transform.forward * movementSpeed);
            }
            //Back
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                newPosition += (transform.forward * -movementSpeed);
            }
            //Right
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                newPosition += (transform.right * movementSpeed);
            }
            //Left
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                newPosition += (transform.right * -movementSpeed);
            }

            //Lerping
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        }
    }


    void RotateCamera()
    {
        if (canMove)
        {
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

            //Rotate Left
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Period))
            {
                newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
            }
            //Rotate right
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Comma))
            {
                newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
            }
        }
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    //Function for handling zooming on camera
    void ZoomCamera()
    {
        if (canMove)
        {
            //Mouse scrolling
            if (Input.mouseScrollDelta.y != 0)
            {
                newZoom += Input.mouseScrollDelta.y * zoomAmount;
            }

            //Increase zoom
            if (Input.GetKey(KeyCode.R))
            {
                newZoom += zoomAmount;
            }

            //Decrease zoom
            if (Input.GetKey(KeyCode.F))
            {
                newZoom -= zoomAmount;
            }

            //Zoom clamping
            newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);
            newZoom.z = Mathf.Clamp(newZoom.z, -maxZoom, -minZoom);

        }
        //Setting cam pos based on zoom
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}