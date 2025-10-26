using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventPopupManager : MonoBehaviour
{
    public static EventPopupManager Instance;

    [Header("References")]
    public GameObject popupRoot;  // Panel v�i popup juur
    public TMP_Text eventText;    // Tekstikast
    public Button okButton;       // Nupp "OK" v�i "Continue"

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

        // Kasuta PauseManagerit m�ngu peatamiseks
        PauseManager.PauseGame();

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            popupRoot.SetActive(false);

            // J�tka m�ngu
            PauseManager.ResumeGame();

            onClose?.Invoke();
        });
    }
}
