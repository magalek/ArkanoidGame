using UnityEngine;

public class Paddle : MonoBehaviour {

    [SerializeField] private Vector2 startPosition;
    [SerializeField] private Vector2 xAxisMovementConstraints;
    [SerializeField] private float speed;

    public float xMovement;

    private bool activated;

    private void Awake() {
        startPosition = transform.position;
    }

    private void Update() {
        if (!activated) return;
        Move();
    }

    private void Move() {
        xMovement = Input.GetAxisRaw("Horizontal");
        float newXPos = transform.position.x + xMovement * speed * Time.deltaTime;
        newXPos = Mathf.Clamp(newXPos, xAxisMovementConstraints.x, xAxisMovementConstraints.y);
        transform.position = new Vector2(newXPos, transform.position.y);
    }

    public void Restart() {
        SetActiveState(true);
        transform.position = startPosition;
    }

    public void SetActiveState(bool state) {
        activated = state;
    }
}
