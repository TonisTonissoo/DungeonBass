using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public Unit player;
    public List<Unit> enemies = new List<Unit>();
    private bool battleActive = true;

    [Header("Final Boss (Loop End)")]
    [SerializeField] private string bossSceneName = "BossFightScene";
    [SerializeField] private string victorySceneName = "Victory";
    [SerializeField] private string gameOverSceneName = "GameOver";

    [Header("Spawning")]
    [Tooltip("Points where enemies will be spawned. If empty, will try to use positions of pre-placed enemies.")]
    public List<Transform> spawnPoints;

    [Tooltip("Default Y height for procedurally spawned enemies (if no spawn points found).")]
    public float defaultSpawnY = -2f;

    [Tooltip("Horizontal spacing between procedurally spawned enemies.")]
    public float spawnSpacing = 3.5f;

    [SerializeField] private float musicVolume = 1f;

    [Header("Scene Load Delay (optional)")]
    [SerializeField] private float loadDelaySeconds = 0.75f;

    void Start()
    {
        SpawnEnemiesFromData();

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

            // ---------------------------
            // LOSS CHECK
            // ---------------------------
            if (!player.IsAlive())
            {
                Debug.Log("Player lost!");

                bool isFinalBossFight = IsFinalBossFight();

                PlayerPrefs.SetString("BattleResult", "Lose");
                PlayerPrefs.Save();

                if (isFinalBossFight)
                {
                    // FINAL BOSS LOSE -> load GameOver scene + music
                    UISoundPlayer.Instance?.PlayDefeat();
                    yield return StartCoroutine(LoadSceneAfterDelay(gameOverSceneName));
                    yield break;
                }

                // Default (nagu enne)
                EndPanelManager.Instance.ShowGameOver();
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

            // ---------------------------
            // VICTORY CHECK
            // ---------------------------
            if (enemies.Count == 0)
            {
                Debug.Log("Player won!");

                int reward = Random.Range(15, 31);
                PlayerStats.Instance.AddCoins(reward);
                Debug.Log($"Player earned {reward} coins!");

                PlayerStats.Instance.HealToFull();
                HUDController.Instance?.UpdateHUD();

                bool isBossScene = SceneManager.GetActiveScene().name == bossSceneName;

                PlayerPrefs.SetString("BattleResult", "Win");
                PlayerPrefs.Save();

                if (isBossScene)
                {
                    // Kui see on boss fight, siis boardi taastamise lipp (nagu sul varem)
                    // AGA final boss win korral pole seda enam vaja, sest me läheme VictoryScene'i
                    bool isFinalBossFight = IsFinalBossFight();

                    if (isFinalBossFight)
                    {
                        // FINAL BOSS WIN -> load Victory scene + music
                        UISoundPlayer.Instance?.PlayVictory();
                        yield return StartCoroutine(LoadSceneAfterDelay(victorySceneName));
                        yield break;
                    }
                    else
                    {
                        PlayerPrefs.SetInt("ReturnAfterBoss", 1);
                        PlayerPrefs.Save();
                    }
                }

                // Default (nagu enne)
                EndPanelManager.Instance.ShowVictory();
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

    private void SpawnEnemiesFromData()
    {
        if (GameLoopManager.Instance != null && GameLoopManager.Instance.nextEncounter != null && GameLoopManager.Instance.nextEncounterCount > 0)
        {
            CombatEncounter encounter = GameLoopManager.Instance.nextEncounter;
            int count = GameLoopManager.Instance.nextEncounterCount;

            // 1. Capture positions from existing enemies if spawnPoints is empty
            if ((spawnPoints == null || spawnPoints.Count == 0) && enemies.Count > 0)
            {
                if (spawnPoints == null) spawnPoints = new List<Transform>();
                foreach (var e in enemies)
                {
                    if (e != null) spawnPoints.Add(e.transform);
                }
            }

            // 2. Clear existing enemies
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] != null) Destroy(enemies[i].gameObject);
            }
            enemies.Clear();

            // 3. Spawn new enemies
            for (int i = 0; i < count; i++)
            {
                // Determine which prefab to use for THIS enemy
                GameObject prefabToSpawn = null;

                // Priority 1: Pick random from list
                if (encounter.enemyPrefabs != null && encounter.enemyPrefabs.Count > 0)
                {
                    prefabToSpawn = encounter.enemyPrefabs[Random.Range(0, encounter.enemyPrefabs.Count)];
                }

                if (prefabToSpawn == null)
                {
                    Debug.LogWarning($"BattleManager: Could not resolve enemy prefab for encounter {encounter.name}");
                    continue;
                }

                Vector3 spawnPos = Vector3.zero;

                if (spawnPoints != null && spawnPoints.Count > 0)
                {
                    // Cycle through spawn points
                    Transform point = spawnPoints[i % spawnPoints.Count];
                    spawnPos = point.position;

                    // Add offset if we are looping through points (multiple enemies on same point)
                    if (i >= spawnPoints.Count)
                    {
                        // Simple offset to avoid perfect overlap.
                        // Assuming 2D, offset on X or Y depending on game style.
                        // We use spawnSpacing / 2 to keep them somewhat grouped but distinct.
                        spawnPos += new Vector3((spawnSpacing * 0.75f) * (i / spawnPoints.Count), 0, 0);
                    }
                }
                else
                {
                    // Procedural fallback: Right side of screen
                    // Start at X=3, then add spacing.
                    spawnPos = new Vector3(3f + (i * spawnSpacing), defaultSpawnY, 0f);
                }

                // Use the PREFAB'S rotation, not Identity. This ensures any pre-configured rotation (like facing left) is preserved.
                GameObject newEnemyObj = Instantiate(prefabToSpawn, spawnPos, prefabToSpawn.transform.rotation);

                // No manual scale manipulation needed in BattleManager. 
                // We rely on the Prefab being set up correctly.

                Unit newEnemyUnit = newEnemyObj.GetComponent<Unit>();

                if (newEnemyUnit != null)
                {
                    enemies.Add(newEnemyUnit);
                    // Rename for clarity in hierarchy
                    newEnemyObj.name = $"{prefabToSpawn.name} {i + 1}";
                }
            }

            Debug.Log($"BattleManager: Spawned {count} enemies from encounter {encounter.name}.");

            // Clear the data so it doesn't persist for other battles (unless intended)
            // Actually, let's keep it in GameLoopManager until overwritten, 
            // but BattleManager consumes it once per Start.
            // It is safer to clear it to prevent accidental respawn on scene reload if logic changes.
            GameLoopManager.Instance.nextEncounter = null;
            GameLoopManager.Instance.nextEncounterCount = 0;
        }
    }

    private bool IsFinalBossFight()
    {
        bool isBossScene = SceneManager.GetActiveScene().name == bossSceneName;

        // loop check: PlayerStats.currentLoop vs GameLoopManager.maxLoops
        bool isFinalLoop =
                GameLoopManager.Instance != null &&
                PlayerStats.Instance != null &&
                GameLoopManager.Instance.IsFinalLoop(PlayerStats.Instance.currentLoop);

        return isBossScene && isFinalLoop;
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName)
    {
        if (loadDelaySeconds > 0f)
            yield return new WaitForSeconds(loadDelaySeconds);

        // Kasuta sinu olemasolevat loaderit
        SceneLoader.Load(sceneName);
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
