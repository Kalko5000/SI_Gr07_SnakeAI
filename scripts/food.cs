using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class food : MonoBehaviour
{
    public BoxCollider2D GridArea;
    public GameObject snake;
    private List<Transform> posSegments;
    private HashSet<Vector2> availability;
    
    void Start()
    {
        availability = new HashSet<Vector2>();
        RandomizePosition();
    }

    void Update()
    {

    }

    private void GetValidPositions() {
        availability.Clear();
        for(int i = 0; i < GridArea.transform.position.x * 2 + 1; i++) {
            for(int j = 0; j < GridArea.transform.position.y * 2 + 1; j++) {
                availability.Add(new Vector2(i, j));
            }
        }
        foreach(Transform segment in posSegments) {
            availability.Remove(new Vector2(segment.position.x, segment.position.y));
        }
    }

    public void RandomizePosition() {
        posSegments = snake.GetComponent<snake>().segments;
        GetValidPositions();
        transform.position = availability.ElementAt(Random.Range(0, availability.Count));
        // Bounds bounds = GridArea.bounds;
        // float x = Random.Range(bounds.min.x, bounds.max.x);
        // float y = Random.Range(bounds.min.y, bounds.max.y);
        // transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0.0f);
    }
}
