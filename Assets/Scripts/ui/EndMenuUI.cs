using UnityEngine;

public class EndMenuUI : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneLoader.Load("Level1");
        Debug.Log("Play Again pressed");
    }

    public void BackToMainMenu()
    {
        SceneLoader.Load("MainMenu");
        Debug.Log("Main Menu pressed");
    }

    public void ContinueGame()
    {
        SceneLoader.Load("Level1");
        Debug.Log("Continue pressed");
    }
}
