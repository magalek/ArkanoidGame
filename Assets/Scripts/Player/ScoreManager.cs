using System;

public class ScoreManager : MonoManager {

    public event Action ScoreChanged;
    public event Action HighScoreChanged;

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    private IDataContainer<int> highscoreContainer = new PlayerPrefsDataContainer<int>(PrefsKeys.HIGHSCORE_KEY);

    private BlockManager blockManager;
    private GameManager gameManager;

    private void Start() {
        blockManager = ManagerService.Instance.Get<BlockManager>();
        gameManager = ManagerService.Instance.Get<GameManager>();
        LoadHighScore();
        blockManager.BlockDestroyed += OnBlockDestroyed;
        gameManager.Restarting += OnRestarting;
        gameManager.Continuing += OnContinuing;
    }

    private void OnRestarting() {
        SetScore(0);
    }

    private void OnContinuing(SaveState state) {
        SetScore(state.Score);
    }

    private void OnBlockDestroyed(BlockBase block) {
        SetScore(Score + block.ScoreReward);
    }

    public void SetScore(int amount) {
        Score = amount;
        ScoreChanged?.Invoke();
        if (Score > HighScore) SetHighScore(Score);
    }

    public void SetHighScore(int amount) {
        HighScore = amount;
        highscoreContainer.Save(HighScore);
        HighScoreChanged?.Invoke();
    }

    private void LoadHighScore() {
        if (highscoreContainer.HasData) SetHighScore(highscoreContainer.Load());
    }

    private void OnDestroy() {
        blockManager.BlockDestroyed -= OnBlockDestroyed;
        gameManager.Restarting -= OnRestarting;
        gameManager.Continuing -= OnContinuing;
    }
}
