using UnityEngine;

public class EndPanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    public static EndPanelManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (victoryPanel) victoryPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        if (victoryPanel)
        {
            UISoundPlayer.Instance?.PlayVictory();

            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel)
        {
            UISoundPlayer.Instance?.PlayDefeat();

            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ReturnToMainMenu()
    {
        UISoundPlayer.Instance.PlayClick();

        Debug.Log("[Run Reset] Returning to Main Menu → resetting run...");

        // Reset PlayerStats
        PlayerStats.Instance.ResetStatsToDefault();

        // Reset GameLoopManager
        if (GameLoopManager.Instance != null)
            GameLoopManager.Instance.SetLoop(1);

        // Clear tile restore prefs
        PlayerPrefs.DeleteKey("LastTileIndex");
        PlayerPrefs.DeleteKey("ReturnAfterBoss");
        PlayerPrefs.DeleteKey("BattleResult");
        PlayerPrefs.Save();

        // Resume time if paused
        Time.timeScale = 1f;

        // Load Main Menu
        FadeController.Instance.FadeToScene("MainMenu");
    }
}
