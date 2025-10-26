using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventPopupManager : MonoBehaviour
{
    public static EventPopupManager Instance;

    [Header("References")]
    public GameObject popupRoot;  // Panel või popup juur
    public TMP_Text eventText;    // Tekstikast
    public Button okButton;       // Nupp "OK" või "Continue"

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        popupRoot.SetActive(false);
    }

    public void ShowEvent(string message, System.Action onClose = null)
    {
        if (popupRoot == null || eventText == null || okButton == null)
        {
            Debug.LogWarning("EventPopupManager is missing UI references!");
            return;
        }

        popupRoot.SetActive(true);
        eventText.text = message;

        // Kasuta PauseManagerit mängu peatamiseks
        PauseManager.PauseGame();

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);

            // Jätka mängu
            PauseManager.ResumeGame();

            onClose?.Invoke();
        });
    }

    public void ShowChoiceEvent(string message, string option1Text, string option2Text, System.Action onPay, System.Action onRisk)
    {
        popupRoot.SetActive(true);
        eventText.text = message;

        okButton.onClick.RemoveAllListeners();


        Button payBtn = Instantiate(okButton, okButton.transform.parent);
        Button riskBtn = Instantiate(okButton, okButton.transform.parent);

        payBtn.GetComponentInChildren<TMP_Text>().text = option1Text;
        riskBtn.GetComponentInChildren<TMP_Text>().text = option2Text;

        okButton.gameObject.SetActive(false); 
        PauseManager.PauseGame();

        payBtn.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);
            Destroy(payBtn.gameObject);
            Destroy(riskBtn.gameObject);
            PauseManager.ResumeGame();
            onPay?.Invoke();
        });

        riskBtn.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);
            Destroy(payBtn.gameObject);
            Destroy(riskBtn.gameObject);
            PauseManager.ResumeGame();
            onRisk?.Invoke();
        });
    }

}
