using UnityEngine;

public static class RunResetter
{
    /// <summary>
    /// FULL RESET: tee seda enne MainMenu laadimist või enne uue run'i alustamist.
    /// Resetib statsid, loopi, PlayerPrefs run-state ja UI (dice panel) kui olemas.
    /// </summary>
    public static void FullReset()
    {
        Debug.Log("[RunResetter] FULL RESET started");

        // 1) Unpause (väga oluline kui tulid end scene'ist)
        Time.timeScale = 1f;

        // 2) Reset PlayerStats (HP/coins/attack jne)
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.ResetStatsToDefault();
            PlayerStats.Instance.currentLoop = 1; // igaks juhuks
        }

        // 3) Reset loop manager
        if (GameLoopManager.Instance != null)
        {
            GameLoopManager.Instance.SetLoop(1);
        }

        // 4) Clear run-state PlayerPrefs
        // (Lisa siia kõik võtmed, mis mõjutavad run'i jätkamist / restore loogikat)
        PlayerPrefs.DeleteKey("LastTileIndex");
        PlayerPrefs.DeleteKey("ReturnAfterBoss");
        PlayerPrefs.DeleteKey("BattleResult");

        // Kui sul tekib tulevikus uusi run-key’sid, lisa need siia:
        // PlayerPrefs.DeleteKey("SomeOtherRunKey");

        PlayerPrefs.Save();

        // 5) Dice UI reset (kui DungeonBoard scene'is olemas)
        ResetDiceUIIfPresent();

        // 6) RESET dice LOGIC state (TurnController static)
        TurnController.ResetDiceState();

        var tc = Object.FindObjectOfType<TurnController>(true);
        if (tc != null)
            tc.ResetRuntimeUI();

        Debug.Log("[RunResetter] FULL RESET finished");
    }

    private static void ResetDiceUIIfPresent()
    {
        DicePanelManager dice = Object.FindObjectOfType<DicePanelManager>(true);
        if (dice == null) return;

        dice.ResetDiceUI();
    }

}
