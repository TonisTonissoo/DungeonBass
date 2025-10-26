using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    public string unitName;
    public float maxHP = 100f;
    public float currentHP;
    public float attackPower = 10f;
    public float attackRange = 1.5f;
    public float moveSpeed = 3f;
    public float attackCooldown = 1.5f;

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
        healthBar.updateHealthBar(currentHP, maxHP);
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

        // hit
        target.TakeDamage(attackPower);
        Debug.Log($"{unitName} hits {target.unitName} for {attackPower} damage!");

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
