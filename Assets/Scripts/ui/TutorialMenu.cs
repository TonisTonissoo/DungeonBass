using TMPro;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject tutorialPanel;
    public GameObject mainButtons;
    public TMP_Text tutorialText;

    private const string DefaultTutorialBody =
        "How to play: \n\n" +
        "Press spacebar to roll the dice and use left click to interact with UI \n\n" +
        "You will have a choice between three different rolls that all have to be used \n\n" +
        "Plan your route carefully and explore different events \n\n" +
        "During gameplay fight enemies, collect coins and visit the shop to make your character stronger \n\n" +
        "At the end of every loop the game gets progressively harder and to win you need to complete 20 loops";

    [TextArea(5, 10)]
    [SerializeField] private string tutorialBody = DefaultTutorialBody;

    void Awake()
    {
        EnsureTutorialBody();
        ApplyText();
    }

    void OnEnable()
    {
        ApplyText();
    }

    void EnsureTutorialBody()
    {
        if (string.IsNullOrWhiteSpace(tutorialBody))
        {
            tutorialBody = DefaultTutorialBody;
        }
    }

    void ApplyText()
    {
        if (tutorialText)
        {
            tutorialText.text = tutorialBody;
        }
    }

    public void OpenTutorial()
    {
        EnsureTutorialBody();
        ApplyText();

        if (mainButtons) mainButtons.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(true);
    }

    public void CloseTutorial()
    {
        if (tutorialPanel) tutorialPanel.SetActive(false);
        if (mainButtons) mainButtons.SetActive(true);
    }
}

