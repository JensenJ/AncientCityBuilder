using UnityEngine;

public class MenuCameraController : MonoBehaviour
{
    [SerializeField] private Vector2 startPosition = new Vector2();
    [SerializeField] private Vector2 endPosition = new Vector2();
    [SerializeField] private Vector2 moveAmount = new Vector2();

    // Start is called before the first frame update
    void Awake()
    {
        //Set start position
        Vector3 position = transform.position;
        position.x = startPosition.x;
        position.z = startPosition.y;
        transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        //Move camera by move amount
        Vector3 position = transform.position;
        position.x += Time.deltaTime * moveAmount.x;
        position.z += Time.deltaTime * moveAmount.y;
        transform.position = position;

        //If reached destination
        if (transform.position.x > endPosition.x || transform.position.z > endPosition.y || Vector3.Distance(transform.position, endPosition) < 1.0f)
        {
            //Set start position
            position = transform.position;
            position.x = startPosition.x;
            position.z = startPosition.y;
            transform.position = position;
        }
    }
}