using UnityEngine;

public class EndMenuUI : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneLoader.Load("DungeonBoard");
        Debug.Log("Play Again pressed");
    }

    public void BackToMainMenu()
    {
        SceneLoader.Load("MainMenu");
        Debug.Log("Main Menu pressed");
    }

    public void ContinueGame()
    {
        SceneLoader.Load("DungeonBoard");
        Debug.Log("Continue pressed");
    }
}
