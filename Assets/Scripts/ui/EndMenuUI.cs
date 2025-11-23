using UnityEngine;

public class EndMenuUI : MonoBehaviour
{
    public void ContinueGame()
    {
        // Continue resumes normal board position
        Time.timeScale = 1f;

        // Mark result for tile restore logic
        PlayerPrefs.SetString("BattleResult", "Win");
        PlayerPrefs.Save();

        // Go back to dungeon board
        SceneLoader.Load("DungeonBoard");
    }

    public void BackToMainMenu()
    {
        Debug.Log("[Run Reset] Back to Main Menu → resetting full run...");

        Time.timeScale = 1f;

        // --- FULL RUN RESET ---

        // Reset PlayerStats to defaults
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ResetStatsToDefault();

        // Reset Loop manager
        if (GameLoopManager.Instance != null)
            GameLoopManager.Instance.SetLoop(1);

        // Clear any tile-based data
        PlayerPrefs.DeleteKey("LastTileIndex");
        PlayerPrefs.DeleteKey("ReturnAfterBoss");
        PlayerPrefs.DeleteKey("BattleResult");
        PlayerPrefs.Save();

        // Load main menu
        SceneLoader.Load("MainMenu");
    }
}
