using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSControlSystem : MonoBehaviour
{

    [SerializeField] GameObject prefabBuilding1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = Utils.GetMousePositionClickData(Camera.main);
            Instantiate(prefabBuilding1, hit.point, Quaternion.identity);
        }
        
    }
}
