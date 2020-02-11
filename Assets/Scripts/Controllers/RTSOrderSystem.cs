using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RTSCameraController))]
public class RTSOrderSystem : MonoBehaviour
{
    //Singleton instance
    public static RTSOrderSystem instance;

    public Transform selectTransform;
    RTSCameraController cameraController = null;
    [SerializeField] LayerMask selectionLayers = new LayerMask();
    [SerializeField] LayerMask targetLayers = new LayerMask();
    // Start is called before the first frame update
    void Start()
    {
        cameraController = GetComponent<RTSCameraController>();
    }

    // Update is called once per frame
    void Update()
    {

        //Select a unit
        if (Input.GetMouseButtonDown(0))
        {
            //Get mouse position
            RaycastHit hit = Utils.GetMousePositionRaycastData(Camera.main, selectionLayers);
            if (hit.collider != null)
            {
                //Set selected transform to transform from collider
                selectTransform = hit.collider.transform;
            }
        }

        //Move a unit
        if (selectTransform != null && Input.GetMouseButtonDown(1))
        {
            //Get mouse position
            RaycastHit hit = Utils.GetMousePositionRaycastData(Camera.main, targetLayers);

            //Get selected unit
            AIUnit unit = selectTransform.GetComponent<AIUnit>();
            if (unit != null)
            {
                //Move unit
                unit.MoveTo(hit.point, 0.1f, null);
            }
            else
            {
                //Log error
                Debug.LogError("Unit is null");
            }
        }

        //Follow toggling
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Toggle follow mode
            if(cameraController.followTransform == null)
            {
                cameraController.followTransform = selectTransform;
            }
            else
            {
                cameraController.followTransform = null;
            }

        }

        //Target following
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Stop following target
            cameraController.followTransform = null;
            selectTransform = null;
        }
    }
}
