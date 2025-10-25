using TMPro;
using UnityEngine;

public class DicePanelManager : MonoBehaviour
{
    public GameObject panelRoot;
    public TextMeshProUGUI headerText;
    public DiceOptionUI[] optionsUI;   // size = 3

    public bool IsVisible => panelRoot && panelRoot.activeSelf;

    public void SetHeader(string text)
    {
        if (headerText) headerText.text = text;
    }

    public void ShowOptions(TurnController.DicePair[] options, bool[] used, System.Action<int> onClick)
    {
        if (!panelRoot || optionsUI == null || optionsUI.Length < 3) return;

        for (int i = 0; i < 3; i++)
        {
            string label = $"{options[i].Sum} ({options[i].a} + {options[i].b})";
            bool interactable = !used[i];
            optionsUI[i].Bind(i, label, interactable, onClick);
            optionsUI[i].gameObject.SetActive(true);
        }

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        if (panelRoot) panelRoot.SetActive(false);
    }
}
