using UnityEngine;

public class EndPanelManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject victoryPanel;
    public GameObject gameOverPanel;

    public static EndPanelManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (victoryPanel) victoryPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        if (victoryPanel)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
