using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaccumSkillBody : SkillBodyBase
{
    private List<Entity> entitiesInRange = new List<Entity>();
    private Dictionary<Entity, float> damageTimers = new Dictionary<Entity, float>();
    private Dictionary<Entity, CoroutineHandle> suckingEntities = new Dictionary<Entity, CoroutineHandle>();

    private struct CoroutineHandle
    {
        public Coroutine coroutine;
        public bool isActive;
    }

    public override void Init(ActiveSkillBase _SkillContext)
    {
        base.Init(_SkillContext);
        useContinuousDamage = true;
    }

    protected override void Update()
    {
        base.Update();

        List<Entity> deadEntities = new List<Entity>();

        foreach (var entity in entitiesInRange)
        {
            if (entity == null || entity.IsDead())
            {
                deadEntities.Add(entity);
                continue;
            }

            if (!damageTimers.ContainsKey(entity))
            {
                damageTimers[entity] = 0f;
            }

            damageTimers[entity] += Time.deltaTime;

            if (damageTimers[entity] >= 1f)
            {
                DealContinuousDamage(entity, skillDamage);
                damageTimers[entity] = 0f;

                if (entity.IsDead())
                {
                    Edible edible = entity.GetComponent<Edible>();
                    if (edible != null)
                    {
                        StartSucking(edible);
                    }
                }
            }
        }

        foreach (var dead in deadEntities)
        {
            entitiesInRange.Remove(dead);
            damageTimers.Remove(dead);
        }
    }

    // ⭐ 올바른 오버라이드!
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // ⭐ 부모의 일반 데미지 로직 무시 (useContinuousDamage = true니까 어차피 안 돌아감)

        if (collision.GetComponent<Worm>() != null) return;

        Entity entity = collision.GetComponent<Entity>();
        if (entity != null && !entitiesInRange.Contains(entity))
        {
            entitiesInRange.Add(entity);
            damageTimers[entity] = 0.99f; // 거의 바로 첫 데미지

            LogHelper.Log($"{entity.name}이(가) 흡입 범위에 진입!");
        }
    }

    // ⭐ 추가: OnTriggerExit2D도 필요
    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            entitiesInRange.Remove(entity);
            damageTimers.Remove(entity);

            LogHelper.Log($"{entity.name}이(가) 흡입 범위에서 벗어남");
        }
    }

    private void StartSucking(Edible edible)
    {
        if (suckingEntities.ContainsKey(edible)) return;

        Coroutine coroutine = StartCoroutine(SuckToMouth(edible));
        suckingEntities[edible] = new CoroutineHandle { coroutine = coroutine, isActive = true };
    }

    private IEnumerator SuckToMouth(Edible edible)
    {
        float elapsed = 0f;
        float duration = 1f;

        Vector3 startPos = edible.transform.position;
        Transform mouth = Worm.Instance.wormHead.transform;

        while (elapsed < duration)
        {
            if (edible == null || mouth == null)
            {
                yield break;
            }

            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            edible.transform.position = Vector3.Lerp(startPos, mouth.position, t);

            yield return null;
        }

        if (edible != null)
        {
            Worm.Instance.GainExp(edible.GetExpReward());
            Worm.Instance.RestoreHunger(edible.GetHungerRestore());

            LogHelper.Log($"{edible.GetEdibleName()}을(를) 흡입해서 먹었다!");

            Destroy(edible.gameObject);
        }

        suckingEntities.Remove(edible);
    }

    private void OnDestroy()
    {
        foreach (var kvp in suckingEntities)
        {
            if (kvp.Value.coroutine != null)
            {
                StopCoroutine(kvp.Value.coroutine);
            }
        }
    }
}