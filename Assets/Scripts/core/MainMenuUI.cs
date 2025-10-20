using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject settingsPanel;
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
        Debug.Log("Play pressed");
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();
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
