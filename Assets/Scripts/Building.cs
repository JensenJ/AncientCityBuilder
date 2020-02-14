using UnityEngine;

//Script to manage all building data such as what units this building produces and also attributes associated with the building.
public class Building : MonoBehaviour
{
    //Building settings
    [SerializeField] private float buildTime = 10.0f;
    [SerializeField] private float buildingHealth = 0.0f;
    [SerializeField] private float maxBuildingHealth = 100.0f;
    [SerializeField] private GameObject scaffoldingPrefab = null;
    [SerializeField] private float scaffoldingSpace = 0.0f;

    //Building variables
    private float buildProgress = 0.0f;
    private bool isBuilding = false;
    private Vector3 buildingEdges;
    private int numberOfBuilders = 0;
    private GameObject[] scaffolding = new GameObject[4];
    private Vector3[] scaffoldingPosition = new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
        //Get building bounds
        Bounds buildingBounds = GetMaxBounds(transform.GetChild(0).gameObject);
        buildingEdges = new Vector3(buildingBounds.size.x, buildingBounds.size.y, buildingBounds.size.z);
    }

    public void CreateScaffolding()
    {
        //Get building bounds
        Bounds buildingBounds = GetMaxBounds(transform.GetChild(0).gameObject);
        buildingEdges = new Vector3(buildingBounds.size.x, buildingBounds.size.y, buildingBounds.size.z);

        //Four sides of building scaffolding
        scaffoldingPosition[0] = new Vector3(buildingEdges.x / 2 + scaffoldingSpace, buildingEdges.y / 2, 0);
        scaffoldingPosition[1] = new Vector3(0, buildingEdges.y / 2, buildingEdges.z / 2 + scaffoldingSpace);
        scaffoldingPosition[2] = new Vector3(-buildingEdges.x / 2 - scaffoldingSpace, buildingEdges.y / 2, 0);
        scaffoldingPosition[3] = new Vector3(0, buildingEdges.y / 2, -buildingEdges.z / 2 - scaffoldingSpace);

        //For every side of building
        for (int i = 0; i < 4; i++)
        {
            scaffolding[i] = Instantiate(scaffoldingPrefab, transform.localPosition + scaffoldingPosition[i], Quaternion.identity, transform);
            
            //If on odd iteration
            if(i % 2 != 0)
            {
                //Scale to fit x side
                scaffolding[i].transform.localScale = new Vector3(buildingEdges.x + scaffoldingSpace, buildingEdges.y, 1.0f);
            }
            else
            {
                //Scale to fit z side
                scaffolding[i].transform.localScale = new Vector3(1.0f, buildingEdges.y, buildingEdges.z + scaffoldingSpace);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfBuilders == 0)
        {
            isBuilding = false;
        }
        else
        {
            isBuilding = true;
        }

        //if has not finished building yet
        if (isBuilding) {
            //Increase build progress
            buildProgress += (Time.deltaTime / buildTime) * numberOfBuilders;

            //Building trnasformation (rise out of ground when building)
            Vector3 buildingPosition = transform.GetChild(0).position;
            buildingPosition.y = 0.0f - ((1 - buildProgress) * buildingEdges.y);

            //Setting building health
            buildingHealth = buildProgress * maxBuildingHealth;

            //If build progress is 1
            if (buildProgress >= 1.0)
            {
                //Finish building
                isBuilding = false;
                buildProgress = 1.0f;
                buildingPosition.y = 0.0f;
                buildingHealth = maxBuildingHealth;
                for (int i = 0; i < 4; i++)
                {
                    Destroy(scaffolding[i]);
                }
            }
            //Update position
            transform.GetChild(0).position = buildingPosition;
        }
    }

    //Function called by building controller when the building is actually placed
    public void AddBuilder()
    {
        numberOfBuilders++;
    }

    public void RemoveBuilder()
    {
        numberOfBuilders--;
    }

    //Getters and Setters
    public float GetBuildingHealth()
    {
        return buildingHealth;
    }

    public float GetMaxBuildingHealth()
    {
        return maxBuildingHealth;
    }

    public void SetBuildingHealth(float health)
    {
        buildingHealth = health;
    }

    //Function to damage building health
    public void DamageBuilding(float damage)
    {
        buildingHealth -= damage;
        //Destroy object if health is zero or below
        if(buildingHealth <= 0)
        {
            Destroy(gameObject);
        }
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
