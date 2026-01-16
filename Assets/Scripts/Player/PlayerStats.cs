using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Singleton
    public static PlayerStats Instance { get; private set; }

    // DEFAULT VALUES (milleni resetitakse)
    public int maxHealthDefault = 100;
    public int attackPowerDefault = 20;
    public int startingCoins = 100;
    public int startingLoop = 1;

    // RUNTIME VALUES (mängus muutuvad)
    public int maxHealth;
    public int currentHealth;
    public int coins;
    public int attackPower;
    public int currentLoop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Reset to defaults at the start of game
        ResetStatsToDefault();
    }

    // Seda kutsub EndPanelManager.ReturnToMainMenu()
    public void ResetStatsToDefault()
    {
        maxHealth = maxHealthDefault;
        currentHealth = maxHealthDefault;

        attackPower = attackPowerDefault;

        coins = startingCoins;
        currentLoop = startingLoop;

        Debug.Log("[PlayerStats] Reset to default stats.");
    }

    // ---- GAMEPLAY STAT FUNCTIONS ---- //

    public void HealToFull()
    {
        currentHealth = maxHealth;
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
        Debug.Log($"Max HP increased to {maxHealth}");
    }

    public void IncreaseAttackPower(int amount)
    {
        attackPower += amount;
        Debug.Log($"Attack Power increased to {attackPower}");
    }

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public bool SpendCoins(int cost)
    {
        if (coins >= cost)
        {
            coins -= cost;
            Debug.Log($"Spent {cost} coins. Remaining: {coins}");
            return true;
        }

        Debug.Log($"Not enough coins! Have {coins}, need {cost}");
        return false;
    }
}
