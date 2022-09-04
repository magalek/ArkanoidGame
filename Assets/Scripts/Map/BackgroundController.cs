using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private float lerptime;

    private BlockManager blockGenerator;
    private SpriteRenderer backgroundRenderer;

    private Coroutine lerpCoroutine;

    private void Awake() {
        backgroundRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        blockGenerator = ManagerService.Instance.Get<BlockManager>();
    }

    private void Start() {
        blockGenerator.GeneratedBlocks += OnBlocksGenerated;
    }

    private void OnBlocksGenerated(List<BlockInfo> blocks) {
        Color newColor = new Color();
        foreach (var blockInfo in blocks) {
            if (blockInfo.Block) newColor += blockInfo.Block.Color;
        }
        newColor /= blocks.Count;
        newColor.a = 1; 
        ChangeBackgroundColor(newColor);
    }

    private void ChangeBackgroundColor(Color newColor) {
        if (lerpCoroutine != null) {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }    
        StartCoroutine(ColorLerpCoroutine());
        IEnumerator ColorLerpCoroutine() {
            Color oldColor = backgroundRenderer.color;
            float t = 0;
            while (t < lerptime) {
                float normalizedT = t / lerptime;
                backgroundRenderer.color = Color.Lerp(oldColor, newColor, normalizedT);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }

    private void OnDestroy() {
        blockGenerator.GeneratedBlocks -= OnBlocksGenerated;
    }
}
