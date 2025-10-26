using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Button buyHealthButton;
    [SerializeField] private Button closeButton;

    [Header("Shop Settings")]
    [SerializeField] private int cost = 50;
    [SerializeField] private int hpIncrease = 25;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Alguses peidetud – ilmub ainult kui mängija satub Shop-ruudule
        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void Start()
    {
        if (buyHealthButton != null)
            buyHealthButton.onClick.AddListener(BuyHealthUpgrade);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);

        UpdateCoinsDisplay();
    }

    public void OpenShop()
    {
        if (panelRoot == null)
        {
            Debug.LogWarning("ShopUI: PanelRoot missing!");
            return;
        }

        panelRoot.SetActive(true);
        PauseManager.PauseGame();

        UpdateCoinsDisplay();
        Debug.Log("Shop opened!");
    }

    public void CloseShop()
    {
        if (panelRoot == null)
        {
            Debug.LogWarning("ShopUI: PanelRoot missing!");
            return;
        }

        panelRoot.SetActive(false);
        PauseManager.ResumeGame();

        Debug.Log("Shop closed.");
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null && PlayerStats.Instance != null)
            coinsText.text = $"Coins: {PlayerStats.Instance.coins}";
    }

    private void BuyHealthUpgrade()
    {
        if (PlayerStats.Instance == null) return;

        if (PlayerStats.Instance.SpendCoins(cost))
        {
            PlayerStats.Instance.IncreaseMaxHealth(hpIncrease);
            Debug.Log($"+{hpIncrease} Max HP purchased for {cost} coins!");

            // Uuenda HUD ja poe UI
            HUDController.Instance?.UpdateHUD();
            UpdateCoinsDisplay();
        }
        else
        {
            Debug.Log("Not enough coins to buy upgrade!");
        }
    }
}
