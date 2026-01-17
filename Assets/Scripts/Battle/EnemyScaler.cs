using UnityEngine;

public class EnemyScaler : MonoBehaviour
{
    public float hpPerLoop = 0.25f;
    public float attackPerLoop = 0.20f;

    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();

        // Move scaling logic to Awake to ensure it happens before Unit.Start() or any other initialization
        if (unit == null) return;

        // 1. Try GameLoopManager
        int loop = 1;
        if (GameLoopManager.Instance != null)
        {
            loop = GameLoopManager.Instance.CurrentLoop;
        }
        // 2. Fallback to PlayerStats
        else if (PlayerStats.Instance != null)
        {
            loop = PlayerStats.Instance.currentLoop;
        }

        // Loop 1 = no scaling
        if (loop <= 1) return;

        // ADJUSTMENT: If this enemy is a minion in the Boss Scene, it should match the Boss's scaling logic
        // (which is Loop - 1, because Loop increments before the fight).
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "BossFightScene")
        {
            loop = Mathf.Max(1, loop - 1);
            if (loop <= 1) return;
        }

        float hpMultiplier = 1f + hpPerLoop * (loop - 1);
        float dmgMultiplier = 1f + attackPerLoop * (loop - 1);

        unit.maxHP = Mathf.RoundToInt(unit.maxHP * hpMultiplier);
        unit.currentHP = unit.maxHP;
        unit.attackPower = Mathf.RoundToInt(unit.attackPower * dmgMultiplier);

        // We don't need to update HealthBar here because Unit.Start() will do it
        // using the new maxHP we just set.

        Debug.Log($"{unit.unitName} scaled for loop {loop}: HP x{hpMultiplier:F2} -> {unit.maxHP}, DMG x{dmgMultiplier:F2} -> {unit.attackPower}");
    }

    // Removed Start() to prevent late updates

}
