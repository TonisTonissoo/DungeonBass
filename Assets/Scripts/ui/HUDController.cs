using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text loopText;

    [Header("Runtime Values")]
    [SerializeField] private int health = 100;
    [SerializeField] private int coins = 0;

    public static HUDController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateHUD();
    }

    public void UpdateHUD()
    {
        if (PlayerStats.Instance != null)
        {
            health = PlayerStats.Instance.currentHealth;
            coins = PlayerStats.Instance.coins;
        }

        // Always read loop from PlayerStats (single source of truth)
        int loop = PlayerStats.Instance != null ? PlayerStats.Instance.currentLoop : 1;
        int maxLoops = 20; // fallback
        if (GameLoopManager.Instance != null)
            maxLoops = GameLoopManager.Instance.MaxLoops;

        if (healthText) healthText.text = $"Health: {health}";
        if (coinsText) coinsText.text = $"Coins: {coins}";
        if (loopText) loopText.text = $"Loop: {loop}/{maxLoops}";
    }

    public void SetHealth(int value)
    {
        health = Mathf.Max(0, value);
        UpdateHUD();
    }

    public void AddCoins(int amount)
    {
        coins = Mathf.Max(0, coins + amount);
        UpdateHUD();
    }
}
