using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceOptionUI : MonoBehaviour
{
    public Button button;                 // can be left null; will auto-find
    public TextMeshProUGUI labelText;     // can be left null; will auto-find
    public Image background;              // optional; will auto-find

    int myIndex = -1;
    System.Action<int> onClicked;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (!background) background = GetComponent<Image>();
        if (!labelText) labelText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Bind(int index, string label, bool interactable, System.Action<int> onClicked)
    {
        myIndex = index;
        this.onClicked = onClicked;

        if (labelText) labelText.text = label;

        if (button)
        {
            button.interactable = interactable;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => this.onClicked?.Invoke(myIndex));
        }

        SetUsedVisual(!interactable);
    }

    public void SetUsedVisual(bool used)
    {
        if (button) button.interactable = !used;

        if (background)
        {
            var c = background.color;
            c.a = used ? 0.5f : 1f;   // darker when used
            background.color = c;
        }
    }
}
