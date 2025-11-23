using TMPro;
using UnityEngine;

public class SpaceHintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    private static SpaceHintUI instance;

    void Awake()
    {
        instance = this;
        if (hintText) hintText.text = "";
    }

    public static void Show(string msg)
    {
        if (instance && instance.hintText)
            instance.hintText.text = msg;
    }

    public static void Hide()
    {
        if (instance && instance.hintText)
            instance.hintText.text = "";
    }
}
