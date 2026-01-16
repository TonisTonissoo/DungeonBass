using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject settingsPanel;

    public void PlayGame()
    {
        UISoundPlayer.Instance.PlayClick();

        PlayerPrefs.DeleteKey("LastTileIndex");
        PlayerPrefs.DeleteKey("BattleResult");
        PlayerPrefs.DeleteKey("LastTileName");
        PlayerPrefs.Save();

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
    }

    public void CloseSettings()
    {
        UISoundPlayer.Instance.PlayClose();

        settingsPanel.SetActive(false);
        mainButtons.SetActive(true);
    }
}
