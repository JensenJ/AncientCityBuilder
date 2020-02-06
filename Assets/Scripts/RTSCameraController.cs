using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Camera controller in a RTS mode
public class RTSCameraController : MonoBehaviour
{
    //Singleton instance
    public static RTSCameraController instance;

    //Transform references
    public Transform cameraTransform;
    public Transform followTransform;

    //Control variables
    [SerializeField] bool canMove = true;
    [SerializeField] bool isFollowingObject = false;
    [SerializeField] [Range(1.0f, 5.0f)] float fastSpeed = 3;
    [SerializeField] [Range(0.1f, 3.0f)] float normalSpeed = 0.5f;
    [SerializeField] [Range(1.0f, 20.0f)] float movementTime = 5;
    [SerializeField] [Range(20.0f, 60.0f)] float minZoom = 40;
    [SerializeField] [Range(80.0f, 400.0f)] float maxZoom = 200;
    [SerializeField] [Range(0.5f, 5.0f)] float rotationAmount = 1.0f;
    [SerializeField] Vector3 zoomAmount = new Vector3(0.0f, -10.0f, 10.0f);

    //Runtime variables
    float movementSpeed;
    Vector3 newPosition;
    Quaternion newRotation;
    Vector3 newZoom;
    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;
    Vector3 rotateStartPosition;
    Vector3 rotateCurrentPosition;

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
        if(followTransform != null && isFollowingObject == true)
        {
            //Set position to AI but allow rotation and zoom
            newPosition = followTransform.position;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
            RotateCamera();
            ZoomCamera();
        }
        else
        {
            //Allow free movement
            PanCamera();
            RotateCamera();
            ZoomCamera();
        }

        //Print mouseclick position
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = Utils.GetMousePositionClickData(Camera.main);
            print(hit.point);
        }

        //Follow toggling
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Toggle follow mode
            isFollowingObject = !isFollowingObject;
        }

        //Target following
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Stop following target
            isFollowingObject = false;
            followTransform = null;
        }
    }

    //Function to pan camera (translate)
    void PanCamera()
    {
        //If allowed to move
        if (canMove)
        {
            //Dragging world with mouse click, panning
            if (Input.GetMouseButtonDown(0))
            {
                //Create plane and raycast it
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    //Get ray hit position
                    dragStartPosition = ray.GetPoint(entry);
                }
            }

            //If still holding mouse down
            if (Input.GetMouseButton(0))
            {
                //Create plane and raycast it
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    //Set position
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

    //Function to rotate camera
    void RotateCamera()
    {
        //If can move
        if (canMove)
        {
            //If holding middle mouse button
            if (Input.GetMouseButtonDown(2))
            {
                rotateStartPosition = Input.mousePosition;
            }
            //If still holding middle mouse button
            if (Input.GetMouseButton(2))
            {
                //Calculate new rotation
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
        //Lerping
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    //Function for handling zooming on camera
    void ZoomCamera()
    {
        //If can move
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