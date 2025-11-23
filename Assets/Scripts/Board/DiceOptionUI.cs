using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceOptionUI : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI labelText;     // shows the sum only
    public Image dieAImage;               // first die
    public Image dieBImage;               // second die
    public Sprite[] diceFaceSprites;      // 6 sprites for faces 1..6

    int myIndex = -1;
    System.Action<int> onClicked;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (!labelText) labelText = GetComponentInChildren<TextMeshProUGUI>(true);
    }

    public void Bind(int index, TurnController.DicePair pair, bool interactable, System.Action<int> onClicked)
    {
        myIndex = index;
        this.onClicked = onClicked;

        // Set text = total
        if (labelText)
            labelText.text = pair.Sum.ToString();

        // Set die faces
        if (diceFaceSprites != null && diceFaceSprites.Length >= 6)
        {
            if (dieAImage && pair.a >= 1 && pair.a <= 6)
                dieAImage.sprite = diceFaceSprites[pair.a - 1];

            if (dieBImage && pair.b >= 1 && pair.b <= 6)
                dieBImage.sprite = diceFaceSprites[pair.b - 1];
        }

        // Button click + enabled state
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
        float alpha = used ? 0.5f : 1f;

        if (button)
            button.interactable = !used;

        if (labelText)
        {
            var c = labelText.color;
            c.a = alpha;
            labelText.color = c;
        }

        if (dieAImage)
        {
            var c = dieAImage.color;
            c.a = alpha;
            dieAImage.color = c;
        }

        if (dieBImage)
        {
            var c = dieBImage.color;
            c.a = alpha;
            dieBImage.color = c;
        }
    }
}
