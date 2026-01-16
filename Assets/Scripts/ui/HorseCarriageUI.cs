using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HorseCarriageUI : MonoBehaviour
{
    public static HorseCarriageUI Instance;

    [Header("UI elements")]
    public GameObject panel;
    public Button skipButton;
    public Button chooseTileButton;
    public TextMeshProUGUI descriptionText;
    public GameObject selectionText;

    [HideInInspector] public bool IsChoosingTile = false;

    private WaypointFollower follower;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void OpenPopup(WaypointFollower f)
    {
        follower = f;

        // Paneel lahti
        UISoundPlayer.Instance.PlayOpen();

        panel.SetActive(true);
        SpaceHintUI.Show("");
        IsChoosingTile = false;

        Debug.Log("[HorseCarriage] Popup opened - waiting for player choice.");

        skipButton.onClick.RemoveAllListeners();
        chooseTileButton.onClick.RemoveAllListeners();

        skipButton.onClick.AddListener(() =>
        {
            // Nupu klikk
            UISoundPlayer.Instance.PlayClick();

            Debug.Log("[HorseCarriage] Player clicked SKIP - event ignored.");
            panel.SetActive(false);

            // Paneel kinni
            UISoundPlayer.Instance.PlayClose();

            if (TurnController.Instance != null)
                TurnController.Instance.UpdateClosedHint();
        });

        chooseTileButton.onClick.AddListener(() =>
        {
            // Nupu klikk
            UISoundPlayer.Instance.PlayClick();

            Debug.Log("[HorseCarriage] Player clicked CHOOSE TILE - entering selection mode.");
            StartTileSelection();
        });
    }

    private void StartTileSelection()
    {
        Debug.Log("[HorseCarriage] Forcing tile selection TRUE");

        // Paneel kinni (läheb selection mode)
        panel.SetActive(false);
        UISoundPlayer.Instance.PlayClose();

        IsChoosingTile = true;

        if (selectionText != null)
            selectionText.SetActive(true);

        Debug.Log("[HorseCarriage] Selection mode ON - waiting for tile click.");
    }

    public void OnTileClicked(Waypoint wp)
    {
        if (!IsChoosingTile)
        {
            Debug.LogWarning("[HorseCarriage] Tile clicked but NOT in selection mode.");
            return;
        }

        // Tile klik heli
        UISoundPlayer.Instance.PlayClick();

        Debug.Log($"[HorseCarriage] Tile clicked: {wp.name} - performing teleport.");
        IsChoosingTile = false;

        follower.TeleportTo(wp);

        TileEvent te = wp.GetComponent<TileEvent>();

        if (te != null)
            Debug.Log($"[HorseCarriage] Triggering tile event: {te.tileType}");
        else
            Debug.LogWarning("[HorseCarriage] No TileEvent found on new tile!");

        te?.TriggerEvent();

        TurnController tc = FindObjectOfType<TurnController>();
        if (tc != null)
        {
            tc.enabled = true;
            Debug.Log("[HorseCarriage] TurnController enabled after teleport.");
        }

        if (selectionText != null)
            selectionText.SetActive(false);

        if (TurnController.Instance != null)
            TurnController.Instance.UpdateClosedHint();
    }

}
