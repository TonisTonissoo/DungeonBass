using System.Collections;
using UnityEngine;

public class BoardCameraController : MonoBehaviour
{
    public static BoardCameraController Instance;

    [Header("Zoom Settings")]
    [Tooltip("How much to zoom in. 0.5 means 50% of original size (Deeper zoom).")]
    public float targetZoomMultiplier = 0.5f;
    [Tooltip("Time it takes to zoom in before fading. Lower is faster.")]
    public float zoomDuration = 0.25f;

    private Camera cam;

    private void Awake()
    {
        // Singleton pattern to ensure easy access from TileEvent
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        cam = GetComponent<Camera>();
    }

    public void TriggerBattleTransition(string sceneName)
    {
        StartCoroutine(ZoomAndLoad(sceneName));
    }

    private IEnumerator ZoomAndLoad(string sceneName)
    {
        // 1. Play Sound
        UISoundPlayer.Instance?.PlayFightStart();

        // 2. Trigger Fade Immediately (0.25s duration)
        if (FadeController.Instance != null)
        {
            FadeController.Instance.FadeToScene(sceneName);
        }

        // 3. Delay before Zoom starts (so Fade gets a head start)
        yield return new WaitForSeconds(0.1f);

        // 4. Zoom In with Acceleration (Ease-In)
        if (cam != null)
        {
            float startSize = cam.orthographicSize;
            float targetSize = startSize * targetZoomMultiplier;
            float elapsed = 0f;

            // We make the zoom slightly shorter/faster than the fade so it feels aggressive
            float currentZoomDuration = zoomDuration;

            while (elapsed < currentZoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / currentZoomDuration);

                // Ease-In Quad (Starts slow, accelerates, ends fast)
                // This gives the "speeding up" feeling without the "stopping" feeling at the end.
                t = t * t;

                cam.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                yield return null;
            }
        }
    }
}
