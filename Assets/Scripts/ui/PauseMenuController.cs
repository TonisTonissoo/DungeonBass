using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused = false;

    private void Update()
    {
        if (PauseManager.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        UISoundPlayer.Instance.PlayOpen();

        pausePanel.SetActive(true);
        PauseManager.PauseGame();
        isPaused = true;
    }

    public void ResumeGame()
    {
        UISoundPlayer.Instance.PlayClose();

        pausePanel.SetActive(false);
        PauseManager.ResumeGame();
        isPaused = false;
    }

    public void OpenSettings()
    {

        Debug.Log("OpenSettings called. settingsPanel=" + settingsPanel);
        UISoundPlayer.Instance.PlayOpen();
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        UISoundPlayer.Instance.PlayClose();

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        UISoundPlayer.Instance.PlayClick();

        PauseManager.ResumeGame();
        SceneLoader.Load("MainMenu");
    }

    public void QuitGame()
    {
        UISoundPlayer.Instance.PlayClick();

        Application.Quit();
    }
}
