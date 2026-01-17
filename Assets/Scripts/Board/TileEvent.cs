using UnityEngine;
using System.Collections.Generic;

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

    [Header("Random Combat Generation")]
    [Tooltip("Add all possible encounters for this tile here. The game will pick one randomly.")]
    public List<CombatEncounter> possibleEncounters;

    [Header("Scaling Settings")]
    [Tooltip("Start adding +1 enemy at this loop (Brackets: 1-6 = +0, 7-14 = +1).")]
    public int tier1LoopThreshold = 7;

    [Tooltip("Start adding +2 enemies at this loop (Brackets: 15-20+ = +2).")]
    public int tier2LoopThreshold = 15;

    [Tooltip("Hard limit on the maximum number of enemies to prevent overcrowding.")]
    public int maxTotalEnemies = 5;

    public void TriggerEvent()
    {
        switch (tileType)
        {
            case TileType.Enemy:
                Debug.Log("Enemy encounter!");
                PlayerPrefs.SetInt("LastTileIndex", transform.GetSiblingIndex());

                if (possibleEncounters != null && possibleEncounters.Count > 0)
                {
                    // 1. Pick a Random Encounter (Random Scene + Random Enemy Type)
                    CombatEncounter selection = possibleEncounters[Random.Range(0, possibleEncounters.Count)];

                    // 2. Roll for Random Amount of Enemies (Bracket Scaling)
                    int currentLoop = GameLoopManager.Instance != null ? GameLoopManager.Instance.CurrentLoop : 1;

                    // Brackets Logic: 1-6 (+0), 7-14 (+1), 15-20+ (+2)
                    int bonus = 0;
                    if (currentLoop >= tier2LoopThreshold)
                    {
                        bonus = 2;
                    }
                    else if (currentLoop >= tier1LoopThreshold)
                    {
                        bonus = 1;
                    }

                    int finalMin = selection.minEnemies + bonus;
                    int finalMax = selection.maxEnemies + bonus;

                    // Safety: Ensure max >= min
                    if (finalMax < finalMin) finalMax = finalMin;

                    // Safety: Clamp to hard limit
                    finalMax = Mathf.Min(finalMax, maxTotalEnemies);
                    finalMin = Mathf.Min(finalMin, finalMax); // Ensure min doesn't exceed capped max

                    int count = Random.Range(finalMin, finalMax + 1);

                    // Clamp to max total enemies
                    count = Mathf.Min(count, maxTotalEnemies);

                    // 3. Save Data for Combat Scene
                    // Use GameLoopManager to pass object references (Prefab)
                    if (GameLoopManager.Instance != null)
                    {
                        GameLoopManager.Instance.nextEncounter = selection;
                        GameLoopManager.Instance.nextEncounterCount = count;
                    }

                    // Keep PlayerPrefs for scene loading and string-based fallbacks
                    PlayerPrefs.SetString("NextCombatScene", selection.combatSceneName);
                    // Deprecated: PlayerPrefs.SetString("NextEnemyType", selection.enemyTypeID); 
                    PlayerPrefs.SetInt("NextEnemyCount", count);
                    PlayerPrefs.Save();

                    Debug.Log($"Generated Encounter: {count} enemies in {selection.combatSceneName} (Loop {currentLoop})");

                    // 4. Load the scene defined in the data
                    UISoundPlayer.Instance?.PlayFightStart();
                    FadeController.Instance.FadeToScene(selection.combatSceneName);
                }
                else
                {
                    Debug.LogError($"Tile {gameObject.name} has no Encounter Data assigned!");
                }
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

                int coinsGained = Random.Range(10, 26);
                PlayerStats.Instance.AddCoins(coinsGained);

                HUDController.Instance?.UpdateHUD();

                if (EventPopupManager.Instance != null)
                    EventPopupManager.Instance.ShowEvent($"You found {coinsGained} coins while resting!");
                else
                    Debug.Log($"You found {coinsGained} coins while resting!");
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
                                EventPopupManager.Instance.ShowEvent($"You paid the bandits {banditCost} coins.");
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

                        // Reduce max HP permanently
                        PlayerStats.Instance.maxHealth = Mathf.Max(1, PlayerStats.Instance.maxHealth - damage);

                        // Clamp current HP to new max
                        PlayerStats.Instance.currentHealth = Mathf.Min(PlayerStats.Instance.currentHealth, PlayerStats.Instance.maxHealth);

                        message = $"A hidden trap injures you! Your max HP decreased by {damage}!";
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

                int loop = PlayerStats.Instance.currentLoop;

                if (loop >= 1)
                {
                    Debug.Log("[StartTile] Boss active -> Launch Boss Scene.");

                    // Save current waypoint index EXACTLY like normal enemy fights
                    PlayerPrefs.SetInt("LastTileIndex", transform.GetSiblingIndex());
                    PlayerPrefs.Save();
                    UISoundPlayer.Instance?.PlayFightStart();
                    FadeController.Instance.FadeToScene("BossFightScene");
                }
                else
                {
                    Debug.Log("[StartTile] No boss yet.");
                }
                break;



        }
    }
}
