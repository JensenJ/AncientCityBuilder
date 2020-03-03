using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUCL.Utilities;

[RequireComponent(typeof(RTSCameraController))]
public class RTSBuildingSystem : MonoBehaviour
{
    [SerializeField] Material ghostValidMat = null;
    [SerializeField] Material ghostInvalidMat = null;
    [SerializeField] LayerMask layersToCheck = new LayerMask();
    [SerializeField] BuildingData[] buildings = null;
    private int selectedBuilding = 0;
    private int lastSelectedBuilding = 0;
    GameObject ghostBuilding = null;
    private float snapGridSize = 1.0f;

    // Update is called once per frame
    void Update()
    {
        //Test for selecting buildings
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            selectedBuilding = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBuilding = 1;
        }
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    selectedBuilding = 2;
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    selectedBuilding = 3;
        //}

        //Range check, checks selected building is valid ID
        if(selectedBuilding >= 1 && selectedBuilding < buildings.Length + 1)
        {
            //Makes sure this code is only executed upon building selection change
            if(lastSelectedBuilding != selectedBuilding)
            {
                //Destroy previous building ghost
                if (ghostBuilding != null)
                {
                    Destroy(ghostBuilding);
                }
                //Get mouse position and spawn new ghost building
                RaycastHit hit = JUCLPhysicsUtils.GetMousePositionRaycastData(Camera.main, layersToCheck);
                ghostBuilding = Instantiate(buildings[selectedBuilding - 1].buildingPrefab, hit.point, Quaternion.identity);
                lastSelectedBuilding = selectedBuilding;
            }
        }

        //If building is selected
        if(ghostBuilding != null)
        {
            //Get mouse position and move object to mouse cursor
            RaycastHit hit = JUCLPhysicsUtils.GetMousePositionRaycastData(Camera.main, layersToCheck);
            //Snap to grid
            if (Input.GetKey(KeyCode.LeftControl))
            {
                //Snap position
                ghostBuilding.transform.position = new Vector3(
                    Mathf.Round(hit.point.x / snapGridSize) + snapGridSize / 2, 
                    Mathf.Round(hit.point.y / snapGridSize) + snapGridSize / 2, 
                    Mathf.Round(hit.point.z / snapGridSize) + snapGridSize / 2);
            }
            else
            {
                //Don't snap to grid
                ghostBuilding.transform.position = hit.point;
            }

            //Rotating the building to be placed.
            if (Input.GetKeyDown(KeyCode.E))
            {
                ghostBuilding.transform.Rotate(0, 45, 0);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ghostBuilding.transform.Rotate(0, -45, 0);
            }
            
            //Resource cost check
            if (buildings[selectedBuilding - 1].goldCost <= GameResources.GetGoldAmount())
            {
                ChangeBuildingMaterial(ghostValidMat);

                //If user clicks to place object
                if (Input.GetMouseButtonDown(0))
                {
                    //Create new object and remove gold from player resources
                    GameObject go = Instantiate(buildings[selectedBuilding - 1].buildingPrefab, ghostBuilding.transform.position, ghostBuilding.transform.rotation);
                    Building building = go.GetComponent<Building>();
                    building.CreateScaffolding();
                    building.AddBuilder();
                    GameResources.RemoveGoldAmount(buildings[selectedBuilding - 1].goldCost);
                    //Destroy ghost building if shift is not held down, also reset selection
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        Destroy(ghostBuilding);
                        selectedBuilding = 0;
                        lastSelectedBuilding = 0;
                    }
                }
            }
            else
            {
                ChangeBuildingMaterial(ghostInvalidMat);
            }

            //Destroy ghostbuilding if escape pressed, reset selection.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(ghostBuilding);
                selectedBuilding = 0;
                lastSelectedBuilding = 0;
            }
        }
    }

    //Function to change all of the materials of a renderer
    void ChangeBuildingMaterial(Material material)
    {
        Renderer[] children;
        children = ghostBuilding.GetComponentsInChildren<Renderer>();
        //For every renderer attached to ghost building
        foreach(Renderer rend in children)
        {
            //Create material array based on length of renderer's material
            Material[] mats = new Material[rend.materials.Length];
            //For every material in that array
            for (int i = 0; i < rend.materials.Length; i++)
            {
                //Set material to defined material
                mats[i] = material;
            }
            //Set array
            rend.materials = mats;
        }
    }
}

//Struct for building data
[System.Serializable]
public struct BuildingData 
{
    public string buildingName;
    public GameObject buildingPrefab;
    public int goldCost;
}
