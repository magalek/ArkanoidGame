using UnityEngine;
using UnityEngine.UI;

public class UILifeIcon : MonoBehaviour {

    [SerializeField] private Sprite fullIcon;
    [SerializeField] private Sprite depletedIcon;

    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void Activate() => image.sprite = fullIcon;
    public void Deactivate() => image.sprite = depletedIcon;
}
