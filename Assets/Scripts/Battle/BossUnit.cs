using UnityEngine;
using UnityEngine.Events;

public class BossUnit : Unit
{
    [Header("Boss Settings")]
    [SerializeField] private int numberOfStages = 3;
    [SerializeField] private BossHealthBar bossHealthBar;

    [Header("Minion System")]
    [SerializeField] private Unit minionPrefab;
    [SerializeField] private Transform[] minionSpawnPoints;
    [SerializeField] private int minionsPerStage = 2;
    [Tooltip("Delay between spawning each minion")]
    [SerializeField] private float minionSpawnDelay = 0.3f;

    public UnityEvent[] onStageChangeEvents;

    private int currentStage;
    private float healthPerStage;

    protected override void Start()
    {
        base.Start();

        currentStage = numberOfStages;
        healthPerStage = maxHP / numberOfStages;

        if (bossHealthBar != null)
        {
            bossHealthBar.SetupStages(numberOfStages);
            bossHealthBar.UpdateHealth(currentHP, maxHP);
        }
    }
       
    public override void TakeDamage(float dmg)
    {
        float healthBeforeDamage = currentHP;

        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        if (bossHealthBar != null && bossHealthBar.gameObject.activeInHierarchy)
        {
            bossHealthBar.UpdateHealth(currentHP, maxHP);
        }

        int stageBeforeDamage = GetStageFromHealth(healthBeforeDamage);
        int stageAfterDamage = GetStageFromHealth(currentHP);
            
        if (stageAfterDamage < stageBeforeDamage)
        {
            currentStage = stageAfterDamage;
            
            if (currentStage > 0)
            {
                TriggerStageChangeEvent(stageBeforeDamage - 1);
                StartCoroutine(SpawnMinions());
            }
            else
            {
                Die();
            }
        }
        else if (currentHP <= 0 && currentStage == 1)
        {
            Die();
        }
    }

    private int GetStageFromHealth(float health)
    {
        if (health <= 0) return 0;
        return Mathf.CeilToInt(health / healthPerStage);
    }

    private void TriggerStageChangeEvent(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < onStageChangeEvents.Length)
        {
            onStageChangeEvents[stageIndex].Invoke();
        }
    }

    private System.Collections.IEnumerator SpawnMinions()
    {
        if (minionPrefab == null)
        {
            Debug.LogWarning("[BossUnit] No minion prefab assigned!");
            yield break;
        }

        BattleManager battleManager = FindObjectOfType<BattleManager>();
        if (battleManager == null)
        {
            Debug.LogError("[BossUnit] BattleManager not found!");
            yield break;
        }

        bool hasSpawnPoints = minionSpawnPoints != null && minionSpawnPoints.Length > 0;
        int spawnCount = hasSpawnPoints ? Mathf.Min(minionsPerStage, minionSpawnPoints.Length) : minionsPerStage;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition;

            if (hasSpawnPoints && i < minionSpawnPoints.Length && minionSpawnPoints[i] != null)
            {
                spawnPosition = minionSpawnPoints[i].position;
            }
            else
            {
                float angle = (360f / minionsPerStage) * i;
                float radius = 2f;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                    0,
                    Mathf.Sin(angle * Mathf.Deg2Rad) * radius
                );
                spawnPosition = transform.position + offset;
            }

            Unit minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            minion.unitName = $"Minion {i + 1}";
            battleManager.enemies.Add(minion);

            if (hitEffectPrefab != null)
            {
                ParticleSystem spawnEffect = Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
                Destroy(spawnEffect.gameObject, 2f);
            }

            yield return new WaitForSeconds(minionSpawnDelay);
        }
    }

    protected override void Die()
    {
        gameObject.SetActive(false);
    }
}