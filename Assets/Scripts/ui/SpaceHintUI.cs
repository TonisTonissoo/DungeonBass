using TMPro;
using UnityEngine;

public class SpaceHintUI : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    private static SpaceHintUI instance;

    private string lastMsg = "";

    void Awake()
    {
        instance = this;
        if (hintText) hintText.text = "";
    }

    public static void Show(string msg)
    {
        if (!instance || !instance.hintText) return;

        msg ??= "";
        instance.lastMsg = msg;          // jäta meelde
        instance.hintText.text = msg;    // näita
    }

    // popup/fight ajal: peida, aga ära unusta
    public static void HideTemporarily()
    {
        if (!instance || !instance.hintText) return;
        instance.hintText.text = "";
    }

    // popup/fight lõpus: too viimane tagasi
    public static void Restore()
    {
        if (!instance || !instance.hintText) return;
        instance.hintText.text = instance.lastMsg;
    }
}
