using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public Unit player;
    public List<Unit> enemies = new List<Unit>();
    private bool battleActive = true;

    void Start()
    {
        // Validate setup
        if (player == null)
        {
            Debug.LogError("BattleManager: Player is not assigned!");
            return;
        }

        if (enemies.Count == 0)
        {
            Debug.LogWarning("BattleManager: No enemies assigned!");
        }

        // Initialize player stats from PlayerStats singleton
        if (PlayerStats.Instance != null && player != null)
        {
            player.maxHP = PlayerStats.Instance.maxHealth;
            player.currentHP = PlayerStats.Instance.currentHealth;
            player.attackPower = PlayerStats.Instance.attackPower;
        }

        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        // Wait one frame to ensure all units are initialized
        yield return null;

        while (battleActive)
        {
            // Check if player is still valid and alive
            if (player == null)
            {
                Debug.LogError("BattleManager: Player reference is null!");
                yield break;
            }

            if (!player.IsAlive())
            {
                Debug.Log("Player lost!");
                EndPanelManager.Instance.ShowGameOver();
                PlayerPrefs.SetString("BattleResult", "Lose");
                PlayerPrefs.Save();
                yield break;
            }

            // Remove dead or null enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] == null || !enemies[i].IsAlive())
                {
                    enemies.RemoveAt(i);
                }
            }

            // Check for victory
            if (enemies.Count == 0)
            {
                Debug.Log("Player won!");

                int reward = Random.Range(15, 31);
                PlayerStats.Instance.AddCoins(reward);
                Debug.Log($"Player earned {reward} coins!");

                PlayerStats.Instance.HealToFull();
                HUDController.Instance?.UpdateHUD();

                EndPanelManager.Instance.ShowVictory();
                PlayerPrefs.SetString("BattleResult", "Win");
                PlayerPrefs.Save();
                yield break;
            }

            // --- Player's Turn ---
            if (enemies.Count > 0 && player != null)
            {
                Unit target = SelectPlayerTarget();
                if (target != null && target.IsAlive())
                {
                    yield return StartCoroutine(player.Attack(target));
                }
            }

            // --- Enemies' Turn ---
            // Create a snapshot to safely iterate even if enemies die
            List<Unit> enemiesToAttack = new List<Unit>(enemies);
            
            foreach (Unit enemy in enemiesToAttack)
            {
                // Ensure both units are still valid and alive
                if (enemy != null && enemy.IsAlive() && player != null && player.IsAlive())
                {
                    yield return StartCoroutine(enemy.Attack(player));
                }
            }

            yield return null;
        }
    }

    private Unit SelectPlayerTarget()
    {
        // Prioritize minions over the boss
        List<Unit> minions = new List<Unit>();
        Unit boss = null;

        foreach (Unit enemy in enemies)
        {
            if (enemy != null && enemy.IsAlive())
            {
                if (enemy is BossUnit)
                {
                    boss = enemy;
                }
                else
                {
                    minions.Add(enemy);
                }
            }
        }

        // Attack a random minion if any exist, otherwise attack the boss
        if (minions.Count > 0)
        {
            return minions[Random.Range(0, minions.Count)];
        }
        else if (boss != null)
        {
            return boss;
        }

        // Fallback to any enemy if categorization fails
        return enemies.Count > 0 ? enemies[0] : null;
    }
}
