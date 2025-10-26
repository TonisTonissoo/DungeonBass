using UnityEngine;

public class EndMenuUI : MonoBehaviour
{
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("DungeonBoard");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("MainMenu");
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        SceneLoader.Load("DungeonBoard"); // hiljem saab muuta "NextLoop"
    }
}
