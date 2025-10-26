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
    public float attackCooldown = 0.5f;

    [Header("Critical Hit Stats")]
    [Range(0f, 1f)]
    public float criticalHitChance = 0.15f;  // 15% chance by default
    public float criticalHitMultiplier = 2f; // Double damage on crit by default

    [Header("VFX")]
    [Tooltip("Particle system prefab to spawn when this unit hits a target.")]
    public ParticleSystem hitEffectPrefab;
    [Tooltip("Optional particle system prefab for critical hit.")]
    public ParticleSystem critEffectPrefab;
    [Tooltip("Vertical offset applied when spawning hit VFX.")]
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
        currentHP = maxHP;
        startPos = transform.position;
        healthBar?.updateHealthBar(currentHP, maxHP);
    }

    public bool IsAlive() => currentHP > 0;

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            currentHP = 0;
            healthBar?.updateHealthBar(currentHP, maxHP);
            Die();
            return;
        }

        healthBar?.updateHealthBar(currentHP, maxHP);
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
        float damage = attackPower;

        if (wasCritical)
        {
            damage *= criticalHitMultiplier;
            Debug.Log($"CRITICAL HIT! {unitName} will deal {damage} damage!");
        }

        return damage;
    }

    private void SpawnHitVFX(bool isCrit, Vector3 position)
    {
        ParticleSystem prefab = isCrit && critEffectPrefab != null ? critEffectPrefab : hitEffectPrefab;
        if (prefab == null) return;

        // spawn instance
        ParticleSystem ps = Instantiate(prefab, position + Vector3.up * hitEffectYOffset, Quaternion.identity);

        // Defensive: ensure the particle system is stopped/cleared before playing once.
        // This avoids double-emission when the prefab has Play On Awake or sub-emitters that auto-play.
        ps.gameObject.SetActive(true);
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Clear(true);
        ps.Play();

        // compute safe lifetime and schedule destroy
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

        // Calculate damage with potential critical hit
        bool wasCrit;
        float damage = CalculateDamage(out wasCrit);

        // hit
        target.TakeDamage(damage);
        Debug.Log($"{unitName} hits {target.unitName} for {damage} damage!");

        // spawn hit VFX at the target position
        SpawnHitVFX(wasCrit, target.transform.position);

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
