using System.Collections.Generic;
using UnityEngine;

public class SandstormBody : SkillBodyBase
{
    // 범위 내 적 추적
    private List<Entity> entitiesInRange = new List<Entity>();
    private Dictionary<Entity, float> damageTimers = new Dictionary<Entity, float>();

    public override void Init(ActiveSkillBase _SkillContext)
    {
        base.Init(_SkillContext);
        useContinuousDamage = true; // 지속 데미지 모드
    }

    protected override void Update()
    {
        base.Update();

        // 범위 내 모든 적에게 1초마다 데미지
        List<Entity> deadEntities = new List<Entity>();

        for (int i = entitiesInRange.Count - 1; i >= 0; i--)
        {
            var entity = entitiesInRange[i];

            if (entity == null || entity.IsDead())
            {
                entitiesInRange.RemoveAt(i);
                damageTimers.Remove(entity);
                continue;
            }

            damageTimers[entity] += Time.deltaTime;

            if (damageTimers[entity] >= 1f)
            {
                DealContinuousDamage(entity, skillDamage);
                damageTimers[entity] = 0f;
            }
        }

        // 죽은 적 제거
        foreach (var dead in deadEntities)
        {
            entitiesInRange.Remove(dead);
            damageTimers.Remove(dead);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Worm>() != null) return;

        Entity entity = collision.GetComponent<Entity>();
        if (entity != null && !entitiesInRange.Contains(entity))
        {
            entitiesInRange.Add(entity);
            damageTimers[entity] = 0.99f; // 거의 바로 첫 데미지

            LogHelper.Log($"{entity.name}이(가) 모래 소용돌이 범위에 진입!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            entitiesInRange.Remove(entity);
            damageTimers.Remove(entity);

            LogHelper.Log($"{entity.name}이(가) 모래 소용돌이 범위에서 벗어남");
        }
    }
}