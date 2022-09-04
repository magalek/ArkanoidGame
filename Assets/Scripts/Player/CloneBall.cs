using UnityEngine;

public class CloneBall : Ball {

    private GameManager gameManager;

    protected override void OnEnable() {
        gameManager = ManagerService.Instance.Get<GameManager>();
    }

    protected override void Start() {
        base.Start();
        gameManager.StartingNew += OnLevelRestart;
        gameManager.Restarting += OnLevelRestart;
    }

    private void OnLevelRestart() {
        Destroy(gameObject);
    }

    public void Push(Vector2 direction) {
        currentSpeed = startSpeed;
        gameObject.SetActive(true);
        rigidbodyRef.isKinematic = false;
        rigidbodyRef.velocity = Vector2.zero;
        rigidbodyRef.AddForce(direction * currentSpeed, ForceMode2D.Impulse);
    }

    protected override bool HitKillTrigger(Collider2D collider) {
        Destroy(gameObject);
        return false;
    }

    private void OnDestroy() {
        gameManager.StartingNew -= OnLevelRestart;
        gameManager.Restarting -= OnLevelRestart;
    }
}
