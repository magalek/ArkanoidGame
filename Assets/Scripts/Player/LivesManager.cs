using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesManager : MonoManager {
    public event Action LivesChanged;
    public event Action LivesDepleted;

    public bool Alive => Lives > 0;
    public int Lives { get; private set; }
    public int MaxLives { get; private set; }

    [SerializeField] private int startingLives;

    private GameManager gameManager;

    protected override void Awake() {
        base.Awake();
        MaxLives = startingLives;
    }

    private void OnEnable() {
        gameManager = ManagerService.Instance.Get<GameManager>();
    }

    private void Start() {
        gameManager.Restarting += OnRestarting;
        gameManager.Continuing += OnContinuing;
    }

    private void OnRestarting() {
        Restart();
    }

    private void OnContinuing(SaveState state) {
        MaxLives = startingLives;
        AddLives(state.Lives);
    }

    public void Restart() {
        MaxLives = startingLives;
        AddLives(MaxLives);
    }

    public void AddLives(int amount) {
        Lives += amount; 
        LivesChanged?.Invoke();
        if (!Alive) LivesDepleted?.Invoke();
    }

    private void OnDestroy() {
        gameManager.Restarting -= OnRestarting;
        gameManager.Continuing -= OnContinuing;
    }
}
