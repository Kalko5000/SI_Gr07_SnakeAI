using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class snake : MonoBehaviour
{
    public Vector2 _direction = Vector2.right;
    public List<Transform> segments = new List<Transform>(); 
    private int score = 0;
    public Transform segmentPrefab;
    public int initialSize = 4;

    public Text scoreText;
    public Text playerText;
    public Button resetButton;
    public Button playerButton;
    public bool humanPlayer = true;

    private float humanSpeed = 0.06f;
    private float AIspeed = 0.01f;
    
    // Start is called before the first frame update
    void Start()
    {
        Time.fixedDeltaTime = humanSpeed;
        ResetState();
        resetButton.onClick.AddListener(ResetState);
        playerButton.onClick.AddListener(SwapPlayer);
    }

    // Update is called once per frame
    void Update()
    {
        if(humanPlayer) {
            if((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _direction != Vector2.down) {
                _direction = Vector2.up;
            } else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && _direction != Vector2.up) {
                _direction = Vector2.down;
            } else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && _direction != Vector2.right) {
                _direction = Vector2.left;
            } else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && _direction != Vector2.left) {
                _direction = Vector2.right;
            } 
        }
    }

    private void FixedUpdate() {
        for(int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }
        
        this.transform.position = new Vector3 (
            Mathf.Round(this.transform.position.x) + _direction.x,
            Mathf.Round(this.transform.position.y) + _direction.y,
            0.0f
        );
    }

    private void Grow() {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);

        score++;
        scoreText.text = score.ToString();
    }

    private void ResetState() {
        for(int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }
        segments.Clear();
        segments.Add(this.transform);
        for(int i = 1; i < initialSize; i++) {
            segments.Add(Instantiate(segmentPrefab));
        }
        this.transform.position = new Vector3(11, 11, 0);

        score = 0;
        scoreText.text = score.ToString();
    }

    private void SwapPlayer() {
        if(humanPlayer) {
            Time.fixedDeltaTime = AIspeed;
            playerText.text = "AI";
        } else {
            Time.fixedDeltaTime = humanSpeed;
            playerText.text = "Human";
        }
        humanPlayer = !humanPlayer;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Food") {
            Grow();
            other.GetComponent<food>().RandomizePosition();
        } else if(other.tag == "Obstacle") {
            ResetState();
        }
    }

    public Transform GetHead() {
        return this.transform;
    }

    public Transform GetTail() {
        return segments[segments.Count - 1].transform;
    }
}
