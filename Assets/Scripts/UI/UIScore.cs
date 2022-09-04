using UnityEngine;
using TMPro;

public class UIScore : MonoBehaviour {

    private const string SCORE_PREFIX = "Score: {0}";
    private const string HIGHSCORE_PREFIX = "Highscore: {0}";

    [SerializeField] private TMP_Text scoreLabel;
    [SerializeField] private TMP_Text highScoreLabel;

    private ScoreManager scoreManager;

    private void OnEnable() {
        scoreManager = ManagerService.Instance.Get<ScoreManager>();
    }

    private void Start() {
        scoreManager.ScoreChanged += OnScoreChanged;
        scoreManager.HighScoreChanged += OnHighScoreChanged;
        OnScoreChanged();
        OnHighScoreChanged();
    }

    private void OnScoreChanged() => scoreLabel.text = string.Format(SCORE_PREFIX, scoreManager.Score);
    private void OnHighScoreChanged() => highScoreLabel.text = string.Format(HIGHSCORE_PREFIX, scoreManager.HighScore);

    private void OnDestroy() {
        scoreManager.ScoreChanged -= OnScoreChanged;
        scoreManager.HighScoreChanged -= OnHighScoreChanged;
    }
}
