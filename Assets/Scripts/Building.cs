using UnityEngine;

//Script to manage all building data such as what units this building produces and also attributes associated with the building.
public class Building : MonoBehaviour
{
    //Building variables
    [SerializeField] private float buildProgress = 0.0f;
    [SerializeField] private bool isBuilding = false;
    [SerializeField] private float buildTime = 10.0f;
    [SerializeField] private Vector3 buildingEdges;
    [SerializeField] private int numberOfBuilders = 0;
    [SerializeField] private float buildingHealth = 0.0f;
    [SerializeField] private float maxBuildingHealth = 100.0f;

    [SerializeField] private GameObject scaffoldingPrefab = null;
    [SerializeField] private GameObject[] scaffolding = new GameObject[4];
    [SerializeField] private Vector3[] scaffoldingPosition = new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
        //Get building bounds
        Bounds buildingBounds = GetMaxBounds(gameObject);
        buildingEdges = new Vector3(buildingBounds.size.x, buildingBounds.size.y, buildingBounds.size.z);
    }

    public void CreateScaffolding()
    {
        //Get building bounds
        Bounds buildingBounds = GetMaxBounds(gameObject);
        buildingEdges = new Vector3(buildingBounds.size.x, buildingBounds.size.y, buildingBounds.size.z);

        //Four sides of building scaffolding
        scaffoldingPosition[0] = new Vector3(buildingEdges.x, buildingEdges.y / 2, 0);
        scaffoldingPosition[1] = new Vector3(0, buildingEdges.y / 2, buildingEdges.z);
        scaffoldingPosition[2] = new Vector3(-buildingEdges.x, buildingEdges.y / 2, 0);
        scaffoldingPosition[3] = new Vector3(0, buildingEdges.y / 2, -buildingEdges.z);

        //For every side of building
        for (int i = 0; i < 4; i++)
        {
            scaffolding[i] = Instantiate(scaffoldingPrefab, transform.position + scaffoldingPosition[i], Quaternion.identity);
            
            if(i % 2 != 0)
            {
                //scaffolding[i].transform.Rotate(0, 90, 0);
                scaffolding[i].transform.localScale = new Vector3(buildingEdges.x, buildingEdges.y, 1.0f);
            }
            else
            {
                scaffolding[i].transform.localScale = new Vector3(1.0f, buildingEdges.y, buildingEdges.z);
            }
            //If on odd iteration
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
            Vector3 buildingPosition = transform.position;
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
            transform.position = buildingPosition;
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
