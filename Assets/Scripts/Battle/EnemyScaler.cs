using UnityEngine;

public class EnemyScaler : MonoBehaviour
{
    public float hpPerLoop = 0.25f;
    public float attackPerLoop = 0.20f;

    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();
    }

    void Start()
    {
        int loop = GameLoopManager.Instance ?
                   GameLoopManager.Instance.CurrentLoop : 1;

        // Loop 1 = no scaling
        if (loop <= 1) return;

        float hpMultiplier = 1f + hpPerLoop * (loop - 1);
        float dmgMultiplier = 1f + attackPerLoop * (loop - 1);

        unit.maxHP *= hpMultiplier;
        unit.currentHP = unit.maxHP;
        unit.attackPower *= dmgMultiplier;

        var hb = unit.GetComponentInChildren<HealthBar>();
        if (hb != null)
            hb.updateHealthBar(unit.currentHP, unit.maxHP);

        Debug.Log($"{unit.unitName} scaled for loop {loop}: HP {unit.maxHP}, DMG {unit.attackPower}");
    }
}
