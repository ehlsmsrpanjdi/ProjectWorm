using UnityEngine;

// ⭐ 추가: 모든 게임 오브젝트의 베이스 클래스
public abstract class Entity : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    [Header("이펙트 (선택)")]
    [SerializeField] protected GameObject deathEffect;
    [SerializeField] protected GameObject hitEffect;

    protected bool isDead = false;

    protected virtual void Awake()
    {
        // ⭐ 추가: 초기화
        currentHealth = maxHealth;
    }

    // ⭐ 추가: 데미지 받기
    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // 히트 이펙트
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        LogHelper.Log($"{gameObject.name}이(가) {damage} 데미지를 받음. 남은 체력: {currentHealth}");

        // 체력이 0 이하면 죽음
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ⭐ 추가: 죽음 처리
    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        LogHelper.Log($"{gameObject.name}이(가) 죽음");

        // 죽음 이펙트
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // ⭐ 추가: Getter
    public float GetMaxHealth() => maxHealth;
    public float GetCurrentHealth() => currentHealth;
    public bool IsDead() => isDead;
}