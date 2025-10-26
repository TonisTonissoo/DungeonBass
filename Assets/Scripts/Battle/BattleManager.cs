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
        StartCoroutine(BattleLoop());
    }

    IEnumerator BattleLoop()
    {
        while (battleActive)
        {
            if (!player.IsAlive())
            {
                EndPanelManager.Instance.ShowGameOver();
                Debug.Log("Player lost!");
                battleActive = false;
                break;
            }

            enemies = enemies.Where(e => e.IsAlive()).ToList();
            if (enemies.Count == 0)
            {
                EndPanelManager.Instance.ShowVictory();
                Debug.Log("Player won!");
                battleActive = false;
                break;
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
