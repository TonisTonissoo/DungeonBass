using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject settingsPanel;
    [SerializeField] private TutorialMenu tutorialMenu;
    public void PlayGame()
    {
        UISoundPlayer.Instance.PlayClick();

        RunResetter.FullReset();

        SceneLoader.Load("DungeonBoard");
        Debug.Log("Play pressed");
    }


    public void QuitGame()
    {
        UISoundPlayer.Instance.PlayClick();

        Debug.Log("Quit pressed");
        SceneLoader.QuitGame();
    }

    public void OpenSettings()
    {
        UISoundPlayer.Instance.PlayOpen();

        mainButtons.SetActive(false);
        settingsPanel.SetActive(true);

        settingsPanel.transform.SetAsLastSibling();
    }

    public void CloseSettings()
    {
        UISoundPlayer.Instance.PlayClose();

        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }

    public void OpenTutorial()
    {
        if (!tutorialMenu)
        {
            Debug.LogError("TutorialMenu reference missing on MainMenuUI.");
            return;
        }

        tutorialMenu.OpenTutorial();
    }

    public void CloseTutorial()
    {
        if (!tutorialMenu)
        {
            Debug.LogError("TutorialMenu reference missing on MainMenuUI.");
            return;
        }

        tutorialMenu.CloseTutorial();
    }

}
