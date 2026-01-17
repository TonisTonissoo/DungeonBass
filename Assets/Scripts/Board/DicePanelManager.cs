using TMPro;
using UnityEngine;

public class DicePanelManager : MonoBehaviour
{
    public GameObject panelRoot;      // DicePanel
    public GameObject rollingRoot;    // RollingRoot (GIF)
    public GameObject optionsRoot;    // OptionsContainer

    public TextMeshProUGUI headerText;
    public DiceOptionUI[] optionsUI;  // Option1, Option2, Option3

    public bool IsVisible => panelRoot && panelRoot.activeSelf;

    void Awake()
    {
        // Kindluse mõttes pane alati startis nulli
        ResetDiceUI();
    }

    public void SetHeader(string text)
    {
        if (headerText) headerText.text = text;
    }

    // Only GIF visible
    public void ShowRolling()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(true);

        if (headerText) headerText.text = "Rolling...";

        if (rollingRoot) rollingRoot.SetActive(true);
        if (optionsRoot) optionsRoot.SetActive(false);
    }

    // Only options visible (called after GIF is done)
    public void ShowOptions(TurnController.DicePair[] options, bool[] used, System.Action<int> onClick)
    {
        if (!panelRoot || optionsUI == null || optionsUI.Length < 3) return;

        if (rollingRoot) rollingRoot.SetActive(false);
        if (optionsRoot) optionsRoot.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            bool interactable = !used[i];
            optionsUI[i].Bind(i, options[i], interactable, onClick);
            optionsUI[i].gameObject.SetActive(true);
        }

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot) panelRoot.SetActive(false);
    }

    // ✅ FULL UI RESET for dice panel
    public void ResetDiceUI()
    {
        // Header
        if (headerText) headerText.text = "";

        // Roots
        if (rollingRoot) rollingRoot.SetActive(false);
        if (optionsRoot) optionsRoot.SetActive(false);

        // Options
        if (optionsUI != null)
        {
            foreach (var opt in optionsUI)
            {
                if (opt == null) continue;

                if (opt.button != null)
                    opt.button.onClick.RemoveAllListeners();

                opt.SetUsedVisual(false);
                opt.gameObject.SetActive(false);
            }
        }

        // Panel off
        if (panelRoot) panelRoot.SetActive(false);
    }
}
