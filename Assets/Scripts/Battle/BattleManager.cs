using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public Unit player;
    public List<Unit> enemies = new List<Unit>();
    private bool battleActive = true;

    void Start()
    {

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
        while (battleActive)
        {
            if (!player.IsAlive())
            {
                Debug.Log("Player lost!");
                EndPanelManager.Instance.ShowGameOver();
                PlayerPrefs.SetString("BattleResult", "Lose");
                PlayerPrefs.Save();
                yield break;
            }

            enemies = enemies.Where(e => e.IsAlive()).ToList();

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

            // Player attacks first
            Unit target = enemies[Random.Range(0, enemies.Count)];
            yield return StartCoroutine(player.Attack(target));

            // Enemies attack back
            foreach (Unit enemy in enemies)
            {
                if (enemy.IsAlive())
                    yield return StartCoroutine(enemy.Attack(player));
            }

            yield return null;
        }
    }
}
