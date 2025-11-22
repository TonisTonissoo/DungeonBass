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
        panel.SetActive(true);
        IsChoosingTile = false;

        Debug.Log("[HorseCarriage] Popup opened — waiting for player choice.");

        skipButton.onClick.RemoveAllListeners();
        chooseTileButton.onClick.RemoveAllListeners();

        skipButton.onClick.AddListener(() =>
        {
            Debug.Log("[HorseCarriage] Player clicked SKIP — event ignored.");
            panel.SetActive(false);
        });

        chooseTileButton.onClick.AddListener(() =>
        {
            Debug.Log("[HorseCarriage] Player clicked CHOOSE TILE — entering selection mode.");
            StartTileSelection();
        });
    }

    private void StartTileSelection()
    {
        Debug.Log("[HorseCarriage] Forcing tile selection TRUE");
        panel.SetActive(false);
        IsChoosingTile = true;
        Debug.Log("[HorseCarriage] Selection mode ON — waiting for tile click.");
        descriptionText.text = "Click any tile on the board!";
    }

    public void OnTileClicked(Waypoint wp)
    {
        if (!IsChoosingTile) 
        {
            Debug.LogWarning("[HorseCarriage] Tile clicked but NOT in selection mode.");
            return;
        }

        Debug.Log($"[HorseCarriage] Tile clicked: {wp.name} — performing teleport.");
        IsChoosingTile = false;

        // Teleport 
        follower.TeleportTo(wp);

        // Event seal ruudul
        TileEvent te = wp.GetComponent<TileEvent>();

        if (te != null)
            Debug.Log($"[HorseCarriage] Triggering tile event: {te.tileType}");
        else
            Debug.LogWarning("[HorseCarriage] No TileEvent found on new tile!");


        te?.TriggerEvent();

        // TurnController tagasi ON
        TurnController tc = FindObjectOfType<TurnController>();
        if (tc != null)
        {
            tc.enabled = true;
            Debug.Log("[HorseCarriage] TurnController enabled after teleport.");
        }

    }
}
