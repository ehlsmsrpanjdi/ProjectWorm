using System.Collections.Generic;
using UnityEngine;

public class SkillBodyBase : MonoBehaviour
{
    protected float skillDuration;
    protected float skillDamage;
    protected float skillScaleRatio;

    // ⭐ 추가: 지속 데미지 옵션
    [SerializeField] protected bool useContinuousDamage = false;
    [SerializeField] protected float damageInterval = 1f; // 몇 초마다 데미지

    // ⭐ 추가: 이미 데미지 받은 적 추적 (일반 데미지용)
    protected HashSet<Entity> damagedEntities = new HashSet<Entity>();

    public virtual void Init(ActiveSkillBase _SkillContext)
    {
        skillDuration = _SkillContext.currentDuration;
        skillDamage = _SkillContext.currentDamage;
        skillScaleRatio = _SkillContext.currentRange;

        transform.localScale *= skillScaleRatio;
    }

    protected virtual void Update()
    {
        skillDuration -= Time.deltaTime;
        if (skillDuration < 0)
        {
            Destroy(gameObject);
        }
    }

    // ⭐ 일반 데미지 (한 번만)
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (useContinuousDamage) return; // 지속 데미지 사용 시 무시

        if (collision.GetComponent<Worm>() != null)
        {
            return;
        }

        Entity entity = collision.GetComponent<Entity>();

        if (entity != null && !damagedEntities.Contains(entity))
        {
            entity.TakeDamage(skillDamage);
            damagedEntities.Add(entity);
        }
    }

    // ⭐ 지속 데미지용 헬퍼 메서드
    protected void DealContinuousDamage(Entity entity, float damage)
    {
        if (entity != null && !entity.IsDead())
        {
            entity.TakeDamage(damage);
        }
    }
}