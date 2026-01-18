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
    [SerializeField] private Button buyAttackButton;
    [SerializeField] private int attackCost = 75;
    [SerializeField] private int attackIncrease = 5;

    [Header("Shop Settings")]
    [SerializeField] private int cost = 50;
    [SerializeField] private int hpIncrease = 25;

    [Header("Potion UI")]
    [SerializeField] private Button buyPotionButton;
    [SerializeField] private TMP_Text potionText;
    [SerializeField] private int potionCost = 25;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panelRoot != null)
            panelRoot.SetActive(false);
    }

    private void Start()
    {
        if (buyHealthButton != null)
            buyHealthButton.onClick.AddListener(BuyHealthUpgrade);

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseShop);

        if (buyAttackButton != null)
            buyAttackButton.onClick.AddListener(BuyAttackUpgrade);

        if (buyPotionButton != null)
            buyPotionButton.onClick.AddListener(BuyPotion);

        UpdateCoinsDisplay();
        UpdatePotionDisplay();
    }

    public void OpenShop()
    {
        if (panelRoot == null)
        {
            Debug.LogWarning("ShopUI: PanelRoot missing!");
            return;
        }

        UISoundPlayer.Instance.PlayOpen();

        panelRoot.SetActive(true);
        PauseManager.PauseGame();
        SpaceHintUI.Show("");

        UpdateCoinsDisplay();
        UpdatePotionDisplay();
        Debug.Log("Shop opened!");
    }

    public void CloseShop()
    {
        if (panelRoot == null)
        {
            Debug.LogWarning("ShopUI: PanelRoot missing!");
            return;
        }

        UISoundPlayer.Instance.PlayClose();

        panelRoot.SetActive(false);
        PauseManager.ResumeGame();

        if (TurnController.Instance != null)
            TurnController.Instance.UpdateClosedHint();

        Debug.Log("Shop closed.");
    }

    private void UpdateCoinsDisplay()
    {
        if (coinsText != null && PlayerStats.Instance != null)
            coinsText.text = $"Coins: {PlayerStats.Instance.coins}";
    }

    private void UpdatePotionDisplay()
    {
        if (potionText != null && PlayerStats.Instance != null)
            potionText.text = $"Potions: {PlayerStats.Instance.healingPotions}";
    }


    private void BuyHealthUpgrade()
    {
        if (PlayerStats.Instance == null) return;

        UISoundPlayer.Instance.PlayClick();
        if (PlayerStats.Instance.SpendCoins(cost))
        {
            UISoundPlayer.Instance.PlayShopBuy();
            PlayerStats.Instance.IncreaseMaxHealth(hpIncrease);
            Debug.Log($"+{hpIncrease} Max HP purchased for {cost} coins!");

            HUDController.Instance?.UpdateHUD();
            UpdateCoinsDisplay();
        }
        else
        {
            UISoundPlayer.Instance.PlayNoMoney();
            Debug.Log("Not enough coins to buy upgrade!");
        }
    }

    private void BuyAttackUpgrade()
    {
        if (PlayerStats.Instance == null) return;

        UISoundPlayer.Instance.PlayClick();
        if (PlayerStats.Instance.SpendCoins(attackCost))
        {
            UISoundPlayer.Instance.PlayShopBuy();
            PlayerStats.Instance.IncreaseAttackPower(attackIncrease);
            Debug.Log($"+{attackIncrease} Attack Power purchased for {attackCost} coins!");

            HUDController.Instance?.UpdateHUD();
            UpdateCoinsDisplay();
        }
        else
        {
            UISoundPlayer.Instance.PlayNoMoney();
            Debug.Log("Not enough coins to buy attack upgrade!");
        }
    }

    private void BuyPotion()
    {
        if (PlayerStats.Instance == null) return;

        UISoundPlayer.Instance.PlayClick();

        if (PlayerStats.Instance.SpendCoins(potionCost))
        {
            UISoundPlayer.Instance.PlayShopBuy();
            PlayerStats.Instance.AddHealingPotion(1);
            HUDController.Instance?.UpdateHUD();
        }
        else
        {
            UISoundPlayer.Instance.PlayNoMoney();
        }

        UpdateCoinsDisplay();
        UpdatePotionDisplay();
    }
}
