using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    public int maxHealth = 100;
    public int currentHealth;
    public int coins = 0;
    public int currentLoop = 1; // kui kasutad Loop HUDis

    private void Awake()
    {
        // ainult üks PlayerStats objekt
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // jääb alles kõikides stseenides
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

    public void AddCoins(int amount)
    {
        coins += amount;
    }

    public bool SpendCoins(int cost)
    {
        if (coins >= cost)
        {
            coins -= cost;
            return true;
        }
        return false;
    }
}
