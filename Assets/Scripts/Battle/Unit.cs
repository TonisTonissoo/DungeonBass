using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    [Header("Base Stats")]
    public string unitName;
    public float maxHP = 100f;
    public float currentHP;
    public float attackPower = 10f;
    public float attackRange = 1.5f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1.5f;

    [Header("Critical Hit")]
    [Range(0f, 1f)]
    [Tooltip("Chance to deal a critical hit (0..1)")]
    public float criticalHitChance = 0.15f;
    [Tooltip("Damage multiplier applied when a critical hit occurs")]
    public float criticalHitMultiplier = 2f;

    [Header("VFX")]
    [Tooltip("Particle system prefab to spawn when this unit hits a target")]
    public ParticleSystem hitEffectPrefab;
    [Tooltip("Optional particle system prefab to spawn on a critical hit")]
    public ParticleSystem critEffectPrefab;
    [Tooltip("Vertical offset for spawning the hit effect")]
    public float hitEffectYOffset = 0.5f;

    private Vector3 startPos;
    private bool isAttacking;

    [SerializeField] HealthBar healthBar;

    private void Awake()
    {
        healthBar = GetComponentInChildren<HealthBar>();
    }

    void Start()
    {
        if (unitName == "Player" && PlayerStats.Instance != null)
        {
            maxHP = PlayerStats.Instance.maxHealth;
            currentHP = PlayerStats.Instance.currentHealth;
            attackPower = PlayerStats.Instance.attackPower; 
        }
        else
        {
            currentHP = maxHP;
        }

        startPos = transform.position;
        healthBar?.updateHealthBar(currentHP, maxHP);
    }

    public bool IsAlive() => currentHP > 0;
    
    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        healthBar?.updateHealthBar(currentHP, maxHP);
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{unitName} died!");
        gameObject.SetActive(false);
    }

    protected bool RollForCriticalHit()
    {
        return Random.value < criticalHitChance;
    }

    protected float CalculateDamage(out bool wasCritical)
    {
        wasCritical = RollForCriticalHit();
        float dmg = attackPower;
        if (wasCritical)
        {
            dmg *= criticalHitMultiplier;
        }
        return dmg;
    }

    private void SpawnHitVFX(bool isCritical, Vector3 position)
    {
        ParticleSystem prefab = isCritical && critEffectPrefab != null ? critEffectPrefab : hitEffectPrefab;
        if (prefab == null) return;

        ParticleSystem ps = Instantiate(prefab, position + Vector3.up * hitEffectYOffset, Quaternion.identity);
        // defensive: ensure a clean start
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Clear(true);
        ps.Play();

        // destroy after estimated lifetime
        var main = ps.main;
        float lifetime = main.duration;
        lifetime += main.startLifetime.constantMax;
        Destroy(ps.gameObject, Mathf.Max(0.1f, lifetime));
    }

    public IEnumerator Attack(Unit target)
    {
        if (isAttacking || !target.IsAlive()) yield break;
        isAttacking = true;

        // move towards target
        Vector3 targetPos = target.transform.position;
        while (Vector3.Distance(transform.position, targetPos) > attackRange)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // calculate damage with critical chance
        bool wasCrit;
        float damage = CalculateDamage(out wasCrit);

        // hit and spawn effect (crit uses critEffectPrefab if assigned)
        target.TakeDamage(damage);
        SpawnHitVFX(wasCrit, target.transform.position);
        if (wasCrit)
            Debug.Log($"CRITICAL! {unitName} hits {target.unitName} for {damage} damage!");
        else
            Debug.Log($"{unitName} hits {target.unitName} for {damage} damage!");

        // move back
        while (Vector3.Distance(transform.position, startPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
}
