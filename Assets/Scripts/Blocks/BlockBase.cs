using System;
using UnityEditor;
using UnityEngine;

public abstract class BlockBase : MonoBehaviour, IDamageable {
    public Color Color => blockColor;
    public int ScoreReward => scoreReward;

    [SerializeField] protected int maxLives;
    [SerializeField] protected int scoreReward;
    [SerializeField] protected Color blockColor;

    private Action<BlockBase> destroyCallback;

    protected int currentLives;

    protected virtual void Awake() { }

    public virtual void Initialize(Action<BlockBase> _destroyCallback) {
        destroyCallback = _destroyCallback;
        currentLives = maxLives;
    }

    public virtual void Damage(int damage) {
        currentLives -= damage;
        if (currentLives <= 0) Destroy();
    }

    public virtual void Destroy() {
        destroyCallback?.Invoke(this);
        Destroy(gameObject);
    }
}
