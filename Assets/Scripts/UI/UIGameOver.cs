using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour, IUIWindow {

    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private GameManager gameManager;
    private SceneManager sceneManager;

    private void Start() {
        gameManager = ManagerService.Instance.Get<GameManager>();
        sceneManager = ManagerService.Instance.Get<SceneManager>();
        restartButton.onClick.AddListener(OnRestartButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
    }

    private void OnRestartButtonClick() {
        gameManager.StartNewGame();
        Time.timeScale = 1;
        Hide();
    }

    private void OnMainMenuButtonClick() {
        Time.timeScale = 1;
        sceneManager.LoadMenu();
    }

    public void Show() {
        Time.timeScale = 0;
        gameObject.SetActive(true);
    }

    public void Hide() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }
}
