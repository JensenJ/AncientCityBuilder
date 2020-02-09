using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSBuildingSystem : MonoBehaviour
{

    [SerializeField] Material ghostValidMat = null;
    [SerializeField] Material ghostInvalidMat = null;
    [SerializeField] LayerMask layersToCheck = new LayerMask();
    [SerializeField] BuildingData[] buildings = null;
    private int selectedBuilding = 1;
    private int lastSelectedBuilding = 1;
    GameObject ghostBuilding = null;

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
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBuilding = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBuilding = 3;
        }

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
                RaycastHit hit = Utils.GetMousePositionRaycastData(Camera.main, layersToCheck);
                ghostBuilding = Instantiate(buildings[selectedBuilding - 1].buildingPrefab, hit.point, Quaternion.identity);
                lastSelectedBuilding = selectedBuilding;
            }
        }

        //If building is selected
        if(ghostBuilding != null)
        {
            //Get mouse position and move object to mouse cursor
            RaycastHit hit = Utils.GetMousePositionRaycastData(Camera.main, layersToCheck);
            ghostBuilding.transform.position = hit.point;
            
            //Resource cost check
            if (buildings[selectedBuilding - 1].goldCost <= GameResources.GetGoldAmount())
            {
                ghostBuilding.GetComponent<MeshRenderer>().material = ghostValidMat;
                //If user clicks to place object
                if (Input.GetMouseButtonDown(0))
                {
                    //Create new object and remove gold from player resources
                    Instantiate(buildings[selectedBuilding - 1].buildingPrefab, hit.point, Quaternion.identity);
                    GameResources.RemoveGoldAmount(buildings[selectedBuilding - 1].goldCost);
                    //Destroy ghost building if shift is not held down.
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        Destroy(ghostBuilding);
                    }
                }
            }
            else
            {
                ghostBuilding.GetComponent<MeshRenderer>().material = ghostInvalidMat;
            }

            //Destroy ghostbuilding if escape pressed.
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(ghostBuilding);
            }
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
