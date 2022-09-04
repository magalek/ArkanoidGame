using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILives : MonoBehaviour
{
    [SerializeField] private UILifeIcon lifeIconPrefab;
    [SerializeField] private RectTransform iconsParent;

    private LivesManager livesManager;
    private List<UILifeIcon> icons = new List<UILifeIcon>();

    private void Start() {
        livesManager = ManagerService.Instance.Get<LivesManager>();
        AddIcons(livesManager.MaxLives);
        livesManager.LivesChanged += OnLivesChanged;
    }

    private void AddIcons(int amount) {
        for (int i = 0; i < amount; i++) {
            icons.Add(Instantiate(lifeIconPrefab, iconsParent));
        }
    }

    private void OnLivesChanged() {
        for (int i = 0; i < livesManager.MaxLives; i++) {
            if (i >= livesManager.Lives) icons[i].Deactivate();
            else icons[i].Activate();
        }
    }

    private void OnDestroy() {
        livesManager.LivesChanged -= OnLivesChanged;
    }
}