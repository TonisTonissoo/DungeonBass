using UnityEngine;
using System.Collections; // Required for Coroutines
using System.Collections.Generic;

[System.Serializable]
public class BoardRegion
{
    public string regionName;
    public Sprite backgroundSprite;
    [Tooltip("The index of the first tile in this region (inclusive).")]
    public int startTileIndex;
    [Tooltip("The index of the last tile in this region (inclusive).")]
    public int endTileIndex;

    // Removed customScale as we will auto-scale to match the Forest (Region 0)
}

public class BoardBackgroundManager : MonoBehaviour
{
    public static BoardBackgroundManager Instance;

    [Header("Configuration")]
    [Tooltip("The SpriteRenderer that displays the board background.")]
    public SpriteRenderer backgroundRenderer;

    [Tooltip("Duration of the cross-fade transition in seconds.")]
    public float fadeDuration = 0.5f;

    [Header("Regions")]
    public List<BoardRegion> regions;

    private Vector3 baseScale;
    private Vector2 baseSpriteSize;
    private SpriteRenderer faderRenderer;
    private Coroutine activeFadeRoutine;

    // Track the intended sprite to prevent redundant fade calls while one is already active
    private Sprite currentTargetSprite;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // 1. Capture the Forest's (First Region's) scale and size as the "Standard"
        if (backgroundRenderer != null && regions.Count > 0 && regions[0].backgroundSprite != null)
        {
            baseScale = backgroundRenderer.transform.localScale;
            baseSpriteSize = regions[0].backgroundSprite.bounds.size;

            // Initialize target sprite to whatever we currently have
            currentTargetSprite = backgroundRenderer.sprite;

            // 2. Create a "Fader" object for smooth transitions
            GameObject faderObj = new GameObject("BackgroundFader");
            faderObj.transform.SetParent(backgroundRenderer.transform.parent); // Sibling
            faderObj.transform.position = backgroundRenderer.transform.position;
            faderObj.transform.rotation = backgroundRenderer.transform.rotation;

            faderRenderer = faderObj.AddComponent<SpriteRenderer>();

            // Match layer settings but put it slightly in front of the BACKGROUND, 
            // but BEHIND everything else (like Tiles, Player, UI).
            // Assuming your background is on "Default" or "Background" layer with low order.
            faderRenderer.sortingLayerID = backgroundRenderer.sortingLayerID;

            // Fix: Force the Fader to be exactly equal to the Background, but rely on Z-depth or draw order.
            // Actually, safest is to match order but offset Z slightly towards camera.
            faderRenderer.sortingOrder = backgroundRenderer.sortingOrder;
            faderObj.transform.position = new Vector3(faderObj.transform.position.x, faderObj.transform.position.y, faderObj.transform.position.z - 0.01f);

            // Start invisible
            faderRenderer.color = new Color(1, 1, 1, 0);
        }
    }

    public void UpdateBackground(int tileIndex, bool immediate = false)
    {
        if (backgroundRenderer == null) return;

        foreach (var region in regions)
        {
            if (tileIndex >= region.startTileIndex && tileIndex <= region.endTileIndex)
            {
                // Check if we need to update (different sprite) OR if we are forced to update immediately (initial setup)
                if (currentTargetSprite != region.backgroundSprite || immediate)
                {
                    // Update our target immediately so subsequent calls in this region are ignored
                    currentTargetSprite = region.backgroundSprite;

                    // Calculate Target Scale for Auto-Scaling
                    Vector3 targetScale = baseScale;

                    if (region.backgroundSprite != null && baseSpriteSize.x > 0)
                    {
                        Vector2 newSize = region.backgroundSprite.bounds.size;
                        float widthRatio = baseSpriteSize.x / newSize.x;
                        float heightRatio = baseSpriteSize.y / newSize.y;

                        targetScale = new Vector3(
                            baseScale.x * widthRatio,
                            baseScale.y * heightRatio,
                            1
                        );
                    }

                    if (immediate)
                    {
                        // Instant Swap
                        if (activeFadeRoutine != null) StopCoroutine(activeFadeRoutine);
                        backgroundRenderer.sprite = region.backgroundSprite;
                        backgroundRenderer.transform.localScale = targetScale;

                        // Ensure fader is hidden
                        if (faderRenderer != null) faderRenderer.color = new Color(1, 1, 1, 0);

                        Debug.Log($"[Background] Instant set to {region.regionName} (Tile {tileIndex})");
                    }
                    else
                    {
                        // Start Smooth Transition
                        if (activeFadeRoutine != null) StopCoroutine(activeFadeRoutine);
                        activeFadeRoutine = StartCoroutine(FadeRoutine(region.backgroundSprite, targetScale));
                        Debug.Log($"[Background] Transitioning to {region.regionName} (Tile {tileIndex})");
                    }
                }
                return;
            }
        }
    }

    private IEnumerator FadeRoutine(Sprite targetSprite, Vector3 targetScale)
    {
        // If fader setup failed, fallback to instant swap
        if (faderRenderer == null)
        {
            backgroundRenderer.sprite = targetSprite;
            backgroundRenderer.transform.localScale = targetScale;
            yield break;
        }

        // 1. Setup Fader with new image
        faderRenderer.sprite = targetSprite;
        faderRenderer.transform.localScale = targetScale;
        faderRenderer.color = new Color(1, 1, 1, 0);

        // 2. Cross-fade: Fade Fader IN
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            faderRenderer.color = new Color(1, 1, 1, t);
            yield return null;
        }

        // 3. Swap: Snap main renderer to new image/scale and hide fader
        backgroundRenderer.sprite = targetSprite;
        backgroundRenderer.transform.localScale = targetScale;
        faderRenderer.color = new Color(1, 1, 1, 0);
    }
}
