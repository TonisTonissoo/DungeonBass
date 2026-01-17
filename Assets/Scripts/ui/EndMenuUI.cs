using UnityEngine;

public class EndMenuUI : MonoBehaviour
{
    public void ContinueGame()
    {
        UISoundPlayer.Instance.PlayClick();

        // Continue resumes normal board position
        Time.timeScale = 1f;

        // Mark result for tile restore logic
        PlayerPrefs.SetString("BattleResult", "Win");
        PlayerPrefs.Save();

        // Go back to dungeon board
        SceneLoader.Load("DungeonBoard");
    }

    public void Respawn()
    {
        UISoundPlayer.Instance.PlayClick();

        RunResetter.FullReset();          // ✅ full reset (stats + loop + prefs)
        SceneLoader.Load("DungeonBoard"); // ✅ uus run
    }

    public void BackToMainMenu()
    {
        UISoundPlayer.Instance.PlayClick();

        Debug.Log("[Run Reset] Back to Main Menu → resetting full run...");

        Time.timeScale = 1f;

        // --- FULL RUN RESET ---

        // Reset PlayerStats to defaults
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ResetStatsToDefault();

        // Reset Loop manager + loop counter
        if (GameLoopManager.Instance != null)
            GameLoopManager.Instance.SetLoop(1);

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.currentLoop = 1;

        // Clear any tile-based data
        PlayerPrefs.DeleteKey("LastTileIndex");
        PlayerPrefs.DeleteKey("ReturnAfterBoss");
        PlayerPrefs.DeleteKey("BattleResult");

        // (optional) kui sul on muid run’i state key’sid, siis lisa siia
        // PlayerPrefs.DeleteKey("SomeOtherRunKey");

        PlayerPrefs.Save();

        // Load main menu
        SceneLoader.Load("MainMenu");
    }

}
