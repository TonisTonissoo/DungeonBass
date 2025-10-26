using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventPopupManager : MonoBehaviour
{
    public static EventPopupManager Instance;

    [Header("References")]
    public GameObject popupRoot;   // Panel või popup juur
    public TMP_Text eventText;     // Tekstikast
    public Button okButton;        // "OK" või "Continue" nupp
    public Button payButton;       // Bandit valiku nupp
    public Button riskButton;      // Bandit valiku nupp

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Peida kõik popupid alguses
        popupRoot.SetActive(false);
        if (payButton != null) payButton.gameObject.SetActive(false);
        if (riskButton != null) riskButton.gameObject.SetActive(false);
        if (okButton != null) okButton.gameObject.SetActive(false);
    }

    // 🟢 Lihtne popup ühe OK-nupuga
    public void ShowEvent(string message, System.Action onClose = null)
    {
        if (popupRoot == null || eventText == null || okButton == null)
        {
            Debug.LogWarning("EventPopupManager is missing UI references!");
            return;
        }

        // Ava popup
        popupRoot.SetActive(true);
        eventText.text = message;

        // Peata mäng
        PauseManager.PauseGame();

        // 🔹 Peida muud nupud
        if (payButton != null) payButton.gameObject.SetActive(false);
        if (riskButton != null) riskButton.gameObject.SetActive(false);

        // 🔹 Näita ainult OK nuppu
        okButton.gameObject.SetActive(true);

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);
            okButton.gameObject.SetActive(false);
            PauseManager.ResumeGame();
            onClose?.Invoke();
        });
    }

    // 🟠 Kahe valikuga popup (nt Bandit event)
    public void ShowChoiceEvent(
        string message,
        string option1Text,
        string option2Text,
        System.Action onPay,
        System.Action onRisk)
    {
        if (popupRoot == null || eventText == null || payButton == null || riskButton == null)
        {
            Debug.LogWarning("EventPopupManager is missing UI references for choice popup!");
            return;
        }

        // Ava popup ja tekst
        popupRoot.SetActive(true);
        eventText.text = message;

        // Peata mäng
        PauseManager.PauseGame();

 
        okButton.gameObject.SetActive(false);
        payButton.gameObject.SetActive(true);
        riskButton.gameObject.SetActive(true);

        payButton.GetComponentInChildren<TMP_Text>().text = option1Text;
        riskButton.GetComponentInChildren<TMP_Text>().text = option2Text;

        payButton.onClick.RemoveAllListeners();
        riskButton.onClick.RemoveAllListeners();

        payButton.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);
            payButton.gameObject.SetActive(false);
            riskButton.gameObject.SetActive(false);
            PauseManager.ResumeGame();
            onPay?.Invoke();
        });

        riskButton.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);
            payButton.gameObject.SetActive(false);
            riskButton.gameObject.SetActive(false);
            PauseManager.ResumeGame();
            onRisk?.Invoke();
        });
    }
}
