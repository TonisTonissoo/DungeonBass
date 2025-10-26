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
    [SerializeField] private int currentLoop = 1;
    [SerializeField] private int totalLoops = 20;

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
            currentLoop = PlayerStats.Instance.currentLoop;
        }

        if (healthText) healthText.text = $"Health: {health}";
        if (coinsText) coinsText.text = $"Coins: {coins}";
        if (loopText) loopText.text = $"Loop: {currentLoop}/{totalLoops}";
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

    public void SetLoop(int current, int total)
    {
        currentLoop = Mathf.Max(1, current);
        totalLoops = Mathf.Max(currentLoop, total);
        UpdateHUD();
    }
}
