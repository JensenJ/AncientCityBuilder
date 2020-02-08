using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSBuildingSystem : MonoBehaviour
{

    [SerializeField] BuildingData[] buildings;
    private int selectedBuilding = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = Utils.GetMousePositionClickData(Camera.main);
            if(buildings[selectedBuilding-1].goldCost <= GameResources.GetGoldAmount())
            {
                Instantiate(buildings[selectedBuilding - 1].buildingPrefab, hit.point, Quaternion.identity);
                GameResources.RemoveGoldAmount(buildings[selectedBuilding - 1].goldCost);
            }
        }
        
    }
}

[System.Serializable]
public struct BuildingData 
{
    public string buildingName;
    public GameObject buildingPrefab;
    public int goldCost;
}
