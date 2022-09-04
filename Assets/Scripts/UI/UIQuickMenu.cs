using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIQuickMenu : MonoBehaviour {

    [SerializeField] private Button backButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private CanvasGroup popupGroup;

    [SerializeField] private float popupFadeTime;
    [SerializeField] private float popupShowTime;

    private GameObject uiContainer;

    private bool shown = false;

    private GameManager gameManager;
    private SceneManager sceneManager;

    private Coroutine popupCoroutine;

    private void Awake() {
        uiContainer = transform.GetChild(0).gameObject;
        uiContainer.SetActive(false);
        popupGroup.alpha = 0;
        backButton.onClick.AddListener(OnBackButtonClicked);
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnEnable() {
        sceneManager = ManagerService.Instance.Get<SceneManager>();
        gameManager = ManagerService.Instance.Get<GameManager>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) ChangeState();
    }

    private void OnBackButtonClicked() {
        ChangeState();
    }

    private void OnSaveButtonClicked() {
        gameManager.SaveCurrentState();
        ShowSavePopup();
    }

    private void OnExitButtonClicked() {
        Time.timeScale = 1;
        sceneManager.LoadMenu();
    }

    private void ChangeState() {
        shown = !shown;
        uiContainer.SetActive(shown);
        Time.timeScale = shown ? 0 : 1;
    }

    private void ShowSavePopup() {
        if (popupCoroutine != null) {
            StopCoroutine(popupCoroutine);
            popupCoroutine = null;
        }
        popupCoroutine = StartCoroutine(PopupCoroutine());
        IEnumerator PopupCoroutine() {
            float t = 0;
            while (t < popupFadeTime) {
                popupGroup.alpha = t / popupFadeTime;
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            yield return new WaitForSecondsRealtime(popupShowTime);
            t = 0;
            while (t < popupFadeTime) {
                popupGroup.alpha = 1 - (t / popupFadeTime);
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            StopCoroutine(popupCoroutine);
            popupCoroutine = null;
        }
    }
}
