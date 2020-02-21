using UnityEngine;

public class WaypointCameraController : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 5.0f)] private float moveSpeed = 0.5f;
    [SerializeField] private bool randomTarget = false;
    [SerializeField] private GameObject[] waypoints = null;
    private int currentWaypoint = 0;

    // Update is called once per frame
    void Update()
    {
        //Null pointer check
        if (waypoints.Length > 0)
        {
            //Check waypoint hasn't been removed or is null
            if (waypoints[currentWaypoint] != null)
            {
                //Set target position
                Vector3 targetPosition = new Vector3(waypoints[currentWaypoint].transform.position.x, 10, waypoints[currentWaypoint].transform.position.z);

                //Calculate direction to new position
                Vector3 moveDir = Vector3.Normalize(targetPosition - transform.position);

                //Move the camera to the next waypoint
                transform.position += moveDir * moveSpeed * Time.deltaTime;

                //If in range of next waypoint
                if (Vector3.Distance(transform.position, targetPosition) < 1.0f)
                {
                    //Get the next target
                    GetNewTarget();
                }
            }
            else
            {
                //Try to find new target
                GetNewTarget();
            }
        }
    }

    //Function to generate a new target position
    void GetNewTarget()
    {
        //If randomly targeting
        if (randomTarget == true)
        {
            //Randomly generate current waypoint index
            currentWaypoint = Random.Range(0, waypoints.Length - 1);
        }
        else
        {
            //Increment waypoint target
            currentWaypoint++;
            //If reached end of waypoint list
            if (currentWaypoint > waypoints.Length - 1)
            {
                //Restart waypoint targeting
                currentWaypoint = 0;
            }
        }
    }
}