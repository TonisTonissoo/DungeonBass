using UnityEngine;

public class BossScaler : MonoBehaviour
{
    [Header("Per loop scaling (stronger than regular enemies)")]
    public float hpPerLoop = 0.40f;
    public float attackPerLoop = 0.30f;
    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    void Start()
    {
        int loop = GameLoopManager.Instance ?
                   GameLoopManager.Instance.CurrentLoop : 1;
        if (loop <= 1) return; // Loop 1 = no scaling

        float hpMultiplier = 1f + hpPerLoop * (loop - 1);
        float dmgMultiplier = 1f + attackPerLoop * (loop - 1);

        unit.maxHP *= hpMultiplier;
        unit.currentHP = unit.maxHP;
        unit.attackPower *= dmgMultiplier;

        var hb = unit.GetComponentInChildren<HealthBar>();
        if (hb != null)
            hb.updateHealthBar(unit.currentHP, unit.maxHP);

        Debug.Log($"{unit.unitName} (Boss) scaled for loop {loop}: HP {unit.maxHP}, DMG {unit.attackPower}");
    }

}
