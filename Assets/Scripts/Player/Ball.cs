using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] protected float startSpeed;
    [SerializeField] protected float speedIncrement;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected int maxPowerPiercing;

    protected float currentSpeed;
    protected int currentDamage = 1;

    protected Rigidbody2D rigidbodyRef;
    protected Vector2 lastVelocity;
    protected Vector2 startPosition;

    protected LivesManager livesManager;

    private bool powerActivated;
    private int remainingPiercing;

    private void Awake() {
        startPosition = transform.position;
        rigidbodyRef = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start() {
        livesManager = ManagerService.Instance.Get<LivesManager>();
    }

    private void LateUpdate() {
        lastVelocity = rigidbodyRef.velocity;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (HitKillTrigger(collider)) return;
        TryDamage(collider);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        TryDamage(collision.collider);

        //currentSpeed = Mathf.Clamp(currentSpeed += speedIncrement, startSpeed, maxSpeed);

        if (collision.collider.CompareTag("Paddle"))
            rigidbodyRef.velocity = GetDirectionFromPaddlePosition(collision.collider.transform) * currentSpeed;


        //Vector2 nextDirection = collision.collider.CompareTag("Paddle")
        //    ? GetDirectionFromPaddlePosition(collision.collider.transform)
        //    : GetDirectionFromCollision(collision);
        //nextDirection = CorrectDirection(nextDirection);
        //rigidbodyRef.velocity = powerActivated && remainingPiercing > 0 ? nextDirection : nextDirection * currentSpeed;
    }

    protected virtual bool HitKillTrigger(Collider2D collider) {
        if (collider.CompareTag("KillTrigger")) {
            livesManager.AddLives(-1);
            if (livesManager.Alive) Restart();
            else {
                gameObject.SetActive(false);
                rigidbodyRef.isKinematic = true;
            }
            return true;
        }
        return false;
    }

    private void TryDamage(Component component) {
        if (component.TryGetComponent(out IDamageable damageable)) {
            damageable.Damage(currentDamage);
            if (powerActivated && remainingPiercing > 0) remainingPiercing--;
            if (remainingPiercing == 0) {
                powerActivated = false;
                currentDamage = 1;
            }
        }
    }

    public void ActivatePower() {
        if (powerActivated) return;
        currentDamage = 100;
        powerActivated = true;
        remainingPiercing = maxPowerPiercing;
    }

    public void Restart() {
        currentSpeed = startSpeed;
        transform.position = startPosition;
        gameObject.SetActive(true);
        rigidbodyRef.isKinematic = false;
        rigidbodyRef.velocity = Vector2.zero;
        rigidbodyRef.AddForce(Vector2.up * currentSpeed, ForceMode2D.Impulse);
    }

    private Vector2 GetDirectionFromPaddlePosition(Transform paddleTransform) {
        if (transform.position.y < paddleTransform.position.y) return Vector2.down;
        Vector2 direction = (transform.position - paddleTransform.position) / transform.localScale.x;
        direction.Normalize();
        direction.y = 1;
        return direction;
    }

    private Vector2 GetDirectionFromCollision(Collision2D collision) {
        if (powerActivated && remainingPiercing > 0) {
            return lastVelocity;
        }
        Vector2 direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal).normalized;
        float positiveY = Mathf.Abs(direction.y);
        direction.y /= positiveY;
        return direction;
    }

    private Vector2 CorrectDirection(Vector2 direction) {
        float yDot = Vector2.Dot(Vector2.up, direction.normalized);
        float xDot = Vector2.Dot(Vector2.right, direction.normalized);
        if (-0.8 > xDot || xDot > 0.8f || Mathf.Approximately(direction.x, 0)) direction.x += Random.Range(0.1f, 0.2f) * Mathf.Sign(direction.x);
        if (-0.8 > yDot || yDot > 0.8f || Mathf.Approximately(direction.y, 0)) direction.y += Random.Range(0.1f, 0.2f) * Mathf.Sign(direction.y);
        
        if (float.IsNaN(direction.x)) direction.x = Mathf.Sign(direction.y);
        if (float.IsNaN(direction.y)) direction.y = Mathf.Sign(direction.x);
        return direction;
    }
}
