using UnityEngine;

public class BossScaler : MonoBehaviour
{
    [Header("Scaling Settings")]
    [Tooltip("Extra HP per loop (0.15 = +15% HP per loop).")]
    public float hpPerLoop = 0.15f;

    [Tooltip("Extra Damage per loop (0.10 = +10% DMG per loop).")]
    public float attackPerLoop = 0.10f;

    [Tooltip("Cap scaling at this loop count (e.g. stop getting harder after Loop 20).")]
    public int maxScalingLoop = 20;

    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
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

        // ADJUSTMENT: Because the Loop Counter increments at the Start Tile *before* the Boss Fight,
        // the "First Boss" occurs when CurrentLoop is 2. 
        // We subtract 1 so the First Boss fights with Loop 1 (Base) stats.
        int difficultyLoop = Mathf.Max(1, loop - 1);

        // Loop 1 (adjusted) = no scaling
        if (difficultyLoop <= 1) return;

        // Cap the loop multiplier effect
        int effectiveLoop = Mathf.Min(difficultyLoop, maxScalingLoop);

        float hpMultiplier = 1f + hpPerLoop * (effectiveLoop - 1);
        float dmgMultiplier = 1f + attackPerLoop * (effectiveLoop - 1);

        unit.maxHP = Mathf.RoundToInt(unit.maxHP * hpMultiplier); // Round to int for clean numbers
        unit.currentHP = unit.maxHP;
        unit.attackPower = Mathf.RoundToInt(unit.attackPower * dmgMultiplier);

        Debug.Log($"{unit.unitName} (Boss) scaled for loop {loop} (Effective {effectiveLoop}): HP x{hpMultiplier:F2} -> {unit.maxHP}, DMG x{dmgMultiplier:F2} -> {unit.attackPower}");
    }

    // Removed Start() as we now do everything in Awake() to ensure stats are ready before BossUnit.Start() calculates stages.

}
