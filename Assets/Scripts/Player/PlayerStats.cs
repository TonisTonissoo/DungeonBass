using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHealth = 100;
    public int currentHealth;
    public int coins = 100;
    public int attackPower = 20;   // baas damage
    public int currentLoop = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

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
