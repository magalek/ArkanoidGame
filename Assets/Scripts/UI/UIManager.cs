using UnityEngine;

public class UIManager : MonoManager {

    public IUIWindow GameOverPanel { get; private set; }

    protected override void Awake() {
        base.Awake();
        GameOverPanel = GetComponentInChildren<UIGameOver>(true);
        GameOverPanel.Hide();
    }
}
