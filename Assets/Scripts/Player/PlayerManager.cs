using UnityEngine;

public class PlayerManager : MonoManager {

    public Paddle Paddle { get; private set; }
    public Ball Ball { get; private set; }

    private GameManager gameManager;

    protected override void Awake() {
        base.Awake();
        Paddle = GetComponentInChildren<Paddle>();
        Ball = GetComponentInChildren<Ball>();
    }

    private void Start() {
        gameManager = ManagerService.Instance.Get<GameManager>();

        gameManager.Restarting += OnRestarting;
        gameManager.StartingNew += OnRestarting;
        gameManager.Continuing += OnContinuing;
        gameManager.Stopping += OnStopping;
    }

    public void ActivatePowerBall() {
        Ball.ActivatePower();
    }

    private void OnStopping() {
        Paddle.SetActiveState(false);
    }

    private void OnRestarting() {
        Paddle.Restart();
        Ball.Restart();
    }

    private void OnContinuing(SaveState state) {
        Paddle.Restart();
        Ball.Restart();
    }

    private void OnDestroy() {
        gameManager.Restarting -= OnRestarting;
        gameManager.StartingNew -= OnRestarting;
        gameManager.Continuing -= OnContinuing;
        gameManager.Stopping -= OnStopping;
    }
}
