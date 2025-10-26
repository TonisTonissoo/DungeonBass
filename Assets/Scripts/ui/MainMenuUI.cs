using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainButtons;
    public GameObject settingsPanel;

    public void PlayGame()
    {
 
        PlayerPrefs.DeleteKey("LastTileIndex");
        PlayerPrefs.DeleteKey("BattleResult");
        PlayerPrefs.DeleteKey("LastTileName");
        PlayerPrefs.Save();

        SceneLoader.Load("DungeonBoard");
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
    