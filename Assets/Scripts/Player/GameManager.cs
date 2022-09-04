using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SaveState {
    public BlocksData BlocksData;
    public int Score;
    public int Lives;

    public SaveState(BlocksData blocksData, int score, int lives) {
        BlocksData = blocksData;
        Score = score;
        Lives = lives;
    }
}

public class GameManager : MonoManager {

    public event Action Stopping;
    public event Action Restarting;
    public event Action StartingNew;
    public event Action<SaveState> Continuing;

    public bool IsPlaying { get; private set; }

    private IDataContainer<bool> continueUsedContainer = new PlayerPrefsDataContainer<bool>(PrefsKeys.CONTINUE_USED__KEY);
    private IDataContainer<SaveState> saveContainer = new PlayerPrefsDataContainer<SaveState>(PrefsKeys.SAVE_STATE_KEY);

    private LivesManager livesManager;
    private ScoreManager scoreManager;
    private BlockManager blockManager;
    private PlayerManager playerManager;

    private UIManager uiGame;

    private IEnumerator Start() {
        livesManager = ManagerService.Instance.Get<LivesManager>();
        scoreManager = ManagerService.Instance.Get<ScoreManager>();
        blockManager = ManagerService.Instance.Get<BlockManager>();
        playerManager = ManagerService.Instance.Get<PlayerManager>();
        uiGame = ManagerService.Instance.Get<UIManager>();

        livesManager.LivesDepleted += OnLivesDepleted;
        yield return null;
        if (continueUsedContainer.HasData && (continueUsedContainer.Load() == true)) LoadGame();
        else StartNewGame();
    }

    private void OnLivesDepleted() {
        Stopping?.Invoke();
        uiGame.GameOverPanel.Show();
        IsPlaying = false;
    }

    public void StartNewGame() {
        Debug.Log("Start Game");
        Restarting?.Invoke();
        IsPlaying = true;
    }

    public void LoadGame() {
        continueUsedContainer.Save(false);
        Continuing?.Invoke(saveContainer.Load());
        IsPlaying = true;
        saveContainer.Delete();
    }

    public void StartNextLevel() {
        StartingNew?.Invoke();
    }

    public void SaveCurrentState() {
        SaveState state = new SaveState(blockManager.GetBlocksData(), scoreManager.Score, livesManager.Lives);
        saveContainer.Save(state);
    }

    private void OnDestroy() {
        livesManager.LivesDepleted -= OnLivesDepleted;
    }
}
