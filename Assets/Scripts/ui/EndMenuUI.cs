using UnityEngine;

public class EndMenuUI : MonoBehaviour
{

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetString("BattleResult", "Win");
        SceneLoader.Load("DungeonBoard");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("MainMenu");
    }
}
