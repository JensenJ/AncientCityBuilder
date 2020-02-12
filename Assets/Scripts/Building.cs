using UnityEngine;

//Script to manage all building data such as what units this building produces and also attributes associated with the building.
public class Building : MonoBehaviour
{
    //Building variables
    [SerializeField] private float buildProgress = 0.0f;
    [SerializeField] private bool isBuilding = false;
    [SerializeField] private float buildTime = 10.0f;
    [SerializeField] private float buildingHeight;

    // Start is called before the first frame update
    void Start()
    {
        //Get building height
        Bounds buildingBounds = GetMaxBounds(gameObject);
        buildingHeight = buildingBounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        //if has not finished building yet
        if (isBuilding) {
            //Increase build progress
            buildProgress += Time.deltaTime / buildTime;

            //Building trnasformation (rise out of ground when building)
            Vector3 buildingPosition = transform.position;
            buildingPosition.y = 0.0f - ((1 - buildProgress) * buildingHeight);

            //If build progress is 1
            if (buildProgress >= 1.0)
            {
                //Finish building
                isBuilding = false;
                buildProgress = 1.0f;
                buildingPosition.y = 0.0f;
            }
            //Update position
            transform.position = buildingPosition;
        }
    }

    //Function called by building controller when the building is actually placed
    public void StartBuilding()
    {
        isBuilding = true;
    }

    //Function to find the bounds of all child objects of a parent gameobject
    Bounds GetMaxBounds(GameObject parent)
    {
        Bounds total = new Bounds(parent.transform.position, Vector3.zero);
        foreach (Renderer child in parent.GetComponentsInChildren<Renderer>())
        {
            total.Encapsulate(child.bounds);
        }
        return total;
    }
}
