using UnityEngine;

public class PowerUpBlock : SimpleBlock {

    [SerializeField] private CloneBall cloneBallPrefab;
    [SerializeField, Range(0, 1f)] private float chanceForCloneBall;
    [SerializeField, Range(0, 1f)] private float chanceForPowerBall;

    private PlayerManager playerManager;

    private void Start() {
        playerManager = ManagerService.Instance.Get<PlayerManager>();
    }

    public override void Destroy() {
        if (Random.value <= chanceForCloneBall) GrantCloneBall();
        if (Random.value <= chanceForPowerBall) GrantPowerBall();
        base.Destroy();
    }

    private void GrantCloneBall() {
        Instantiate(cloneBallPrefab, transform.position, Quaternion.identity).Push(Vector2.up);
    }

    private void GrantPowerBall() {
        playerManager.ActivatePowerBall();
    }
}