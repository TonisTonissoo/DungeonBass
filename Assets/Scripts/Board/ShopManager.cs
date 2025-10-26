using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OpenShop()
    {
        // Näita poe UI-d (võime lisada hiljem)
        Debug.Log("Shop opened!");
    }

    public void BuyHealthUpgrade(int cost, int healthIncrease)
    {
        if (PlayerStats.Instance.SpendCoins(cost))
        {
            PlayerStats.Instance.IncreaseMaxHealth(healthIncrease);
            Debug.Log($"+{healthIncrease} max HP ostetud!");
        }
        else
        {
            Debug.Log("Pole piisavalt münte!");
        }
    }
}
