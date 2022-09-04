using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenuManager : MonoManager
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    private IDataContainer<SaveState> saveContainer = new PlayerPrefsDataContainer<SaveState>(PrefsKeys.SAVE_STATE_KEY);
    private IDataContainer<bool> continueUsedContainer = new PlayerPrefsDataContainer<bool>(PrefsKeys.CONTINUE_USED__KEY);

    private SceneManager sceneManager;

    private void OnEnable() {
        sceneManager = ManagerService.Instance.Get<SceneManager>();
    }

    private void Start() {
        continueButton.interactable = saveContainer.HasData;
        playButton.onClick.AddListener(OnPlayButtonClicked);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnPlayButtonClicked() {
        sceneManager.LoadGame();
    }

    private void OnContinueButtonClicked() {
        continueUsedContainer.Save(true);
        sceneManager.LoadGame();
    }

    private void OnExitButtonClicked() {
        Application.Quit();
    }

    
}
