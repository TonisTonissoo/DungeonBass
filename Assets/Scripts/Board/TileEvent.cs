using UnityEngine;

public enum TileType
{
    Enemy,
    Shop,
    Rest,
    Bandit,
    HorseCarriage,
    RandomEvent,
    Boss,
    Start
}

public class TileEvent : MonoBehaviour
{
    public TileType tileType;

    public void TriggerEvent()
    {
        switch (tileType)
        {
            case TileType.Enemy:
                Debug.Log("Enemy encounter!");
                PlayerPrefs.SetInt("LastTileIndex", transform.GetSiblingIndex());
                PlayerPrefs.Save();
                FadeController.Instance.FadeToScene("CombatScene");
                break;

            case TileType.Boss:
                Debug.Log("Boss fight!");
                PlayerPrefs.SetInt("LastTileIndex", transform.GetSiblingIndex());
                PlayerPrefs.Save();
                SceneLoader.Load("CombatScene");
                break;

            case TileType.Shop:
                Debug.Log("Shop entered!");
                if (ShopUI.Instance != null)
                    ShopUI.Instance.OpenShop();
                else
                    Debug.LogWarning("ShopUI.Instance is missing in the scene!");
                break;

            case TileType.Rest:
                Debug.Log("Rest event triggered!");

                int coinsGained = Random.Range(10, 26); // 10–25 münti
                PlayerStats.Instance.AddCoins(coinsGained);

                HUDController.Instance?.UpdateHUD();

                // Kui sul on EventPopupManager (popup tekstiks)
                if (EventPopupManager.Instance != null)
                {
                    EventPopupManager.Instance.ShowEvent($"You found {coinsGained} coins while resting!");
                }
                else
                {
                    Debug.Log($"You found {coinsGained} coins while resting!");
                }
                break;

            case TileType.Bandit:
                Debug.Log("Bandit event triggered!");

                int banditCost = 30;
                int riskLoss = 60;

                if (EventPopupManager.Instance != null)
                {
                    EventPopupManager.Instance.ShowChoiceEvent(
                        $"Bandits block your path!\nPay {banditCost} coins or risk losing {riskLoss}?",
                        "Pay",
                        "Risk",
                        onPay: () =>
                        {
                            if (PlayerStats.Instance != null && PlayerStats.Instance.coins >= banditCost)
                            {
                                PlayerStats.Instance.SpendCoins(banditCost);
                                HUDController.Instance?.UpdateHUD();
                                EventPopupManager.Instance.ShowEvent($"You paid the bandits {banditCost} coins and they let you pass.");
                            }
                            else
                            {
                                EventPopupManager.Instance.ShowEvent("You don't have enough coins! The bandits take all your remaining gold!");
                                PlayerStats.Instance.coins = 0;
                                HUDController.Instance?.UpdateHUD();
                            }
                        },
                        onRisk: () =>
                        {
                            bool lost = Random.value < 0.5f;
                            if (lost)
                            {
                                int amount = Mathf.Min(PlayerStats.Instance.coins, riskLoss);
                                PlayerStats.Instance.coins -= amount;
                                EventPopupManager.Instance.ShowEvent($"You tried to resist, but the bandits took {amount} coins!");
                            }
                            else
                            {
                                EventPopupManager.Instance.ShowEvent("You managed to scare the bandits away! You keep your coins!");
                            }

                            HUDController.Instance?.UpdateHUD();
                        }
                    );
                }
                else
                {
                    Debug.LogWarning("EventPopupManager not found! (Bandit event fallback)");
                }
                break;



            case TileType.HorseCarriage:
                Debug.Log("[TileEvent] HorseCarriage event triggered.");

                // 1) STOP ALL DICE INPUT
                TurnController tc = FindObjectOfType<TurnController>();
                if (tc != null)
                {
                    tc.enabled = false;
                    Debug.Log("[TileEvent] TurnController disabled for teleport selection.");
                }

                // 2) OPEN POPUP
                var f = FindObjectOfType<WaypointFollower>();
                HorseCarriageUI.Instance.OpenPopup(f);

                break;




            case TileType.RandomEvent:
                Debug.Log("Random event triggered!");

                int eventIndex = Random.Range(0, 5); // 5 sündmust
                string message = "";

                switch (eventIndex)
                {
                    // Head sündmused
                    case 0:
                        int treasure = Random.Range(20, 51);
                        PlayerStats.Instance.AddCoins(treasure);
                        message = $"You found a hidden chest of gold! (+{treasure} coins)";
                        break;

                    case 1:
                        int hpBoost = Random.Range(10, 26);
                        PlayerStats.Instance.IncreaseMaxHealth(hpBoost);
                        message = $"You found a healing fountain! Your max HP increased by {hpBoost}.";
                        break;

                    // Halvad sündmused
                    case 2:
                        int damage = Random.Range(15, 31);
                        PlayerStats.Instance.currentHealth = Mathf.Max(0, PlayerStats.Instance.currentHealth - damage);
                        message = $"A hidden trap injures you! (-{damage} HP)";
                        break;

                    case 3:
                        int coinLoss = Random.Range(10, 26);
                        PlayerStats.Instance.coins = Mathf.Max(0, PlayerStats.Instance.coins - coinLoss);
                        message = $"That chest was a mimic! You lost {coinLoss} coins.";
                        break;

                    // Eriline sündmus — Travel to Shop
                    case 4:
                        message = "A mysterious portal sends you directly to a shop!";
                        if (EventPopupManager.Instance != null)
                        {
                            EventPopupManager.Instance.ShowEvent(message, () =>
                            {
                                ShopUI.Instance?.OpenShop();
                            });
                        }
                        else
                        {
                            Debug.Log(message);
                            ShopUI.Instance?.OpenShop();
                        }
                        break;
                }

                HUDController.Instance?.UpdateHUD();

                if (eventIndex != 4)
                {
                    if (EventPopupManager.Instance != null)
                        EventPopupManager.Instance.ShowEvent(message);
                    else
                        Debug.Log(message);
                }

                break;


            case TileType.Start:
                Debug.Log("Start tile.");
                break;
        }
    }
}
