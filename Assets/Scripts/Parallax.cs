using UnityEngine;

public class Parallax : MonoBehaviour
{

    public Camera your_camera;
    public float parallax_value;
    public float yOffset;
    Vector2 length;

    Vector3 startposition;
    // Start is called before the first frame update
    void Start()
    {
        startposition = transform.position;
        startposition = new Vector3(startposition.x, startposition.y + yOffset, startposition.z);
        length = GetComponentInChildren<SpriteRenderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 relative_pos = your_camera.transform.position * parallax_value;
        Vector3 dist = your_camera.transform.position - relative_pos;
        if (dist.x > startposition.x + (length.x))
        {
            Debug.Log("1");
            startposition.x += length.x;
        }
        if (dist.x < startposition.x - (length.x))
        {
            Debug.Log("2");
            startposition.x -= length.x;
        }
        relative_pos.z = startposition.z;
        transform.position = startposition + relative_pos;

    }
}