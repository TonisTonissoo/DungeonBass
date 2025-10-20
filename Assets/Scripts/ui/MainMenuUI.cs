using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject settingsPanel;

    public void PlayGame()
    {
        SceneLoader.Load("Level1");
        Debug.Log("Play pressed");
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        SceneLoader.QuitGame();
    }

    public void OpenSettings()
    {
        mainButtons.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }
}
    