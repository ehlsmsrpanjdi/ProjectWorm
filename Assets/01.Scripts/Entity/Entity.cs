using Unity.VisualScripting;
using UnityEngine;

// ⭐ 수정: Worm은 health를 사용하지 않음 (Hunger가 체력)
public abstract class Entity : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] protected float maxHealth = 10f;
    protected float currentHealth;

    [Header("이펙트 (선택)")]
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] protected GameObject hitEffect;

    protected bool isDead = false;

    [SerializeField] protected Rigidbody2D rb;

    [SerializeField] protected BoxCollider2D col;

    protected virtual void Reset()
    {
        rb = GetComponentInChildren<Rigidbody2D>();
        if (rb == null)
        {
            rb = transform.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
        col = GetComponentInChildren<BoxCollider2D>();
        if (col == null)
        {
            col = transform.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
        }
    }

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        LogHelper.Log($"{gameObject.name}이(가) {damage} 데미지를 받음. 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        LogHelper.Log($"{gameObject.name}이(가) 죽음");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

    }

    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public bool IsDead() => isDead;
}