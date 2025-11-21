using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaccumSkillBody : SkillBodyBase
{
    private List<Entity> entitiesInRange = new List<Entity>();
    private Dictionary<Entity, float> damageTimers = new Dictionary<Entity, float>();
    private Dictionary<Entity, Coroutine> suckingEntities = new Dictionary<Entity, Coroutine>();

    // TriggerExit에서 표시만 하고 실제 삭제는 Update에서 처리
    private HashSet<Entity> pendingRemove = new HashSet<Entity>();

    public override void Init(ActiveSkillBase _SkillContext)
    {
        base.Init(_SkillContext);
        useContinuousDamage = true;
    }

    protected override void Update()
    {
        base.Update();

        // 1) TriggerExit로 표시된 애들 먼저 제거
        if (pendingRemove.Count > 0)
        {
            foreach (var e in pendingRemove)
            {
                entitiesInRange.Remove(e);
                damageTimers.Remove(e);
            }
            pendingRemove.Clear();
        }

        // 2) 지속 데미지 계산 (역순 안전)
        for (int i = entitiesInRange.Count - 1; i >= 0; i--)
        {
            var entity = entitiesInRange[i];

            if (entity == null || entity.IsDead())
            {
                // 사망 → 흡입 시작 → 리스트 제거
                HandleDeadEntity(entity);
                continue;
            }

            // 타이머 업데이트
            if (!damageTimers.ContainsKey(entity))
                damageTimers[entity] = 0f;

            damageTimers[entity] += Time.deltaTime;

            // 1초 데미지
            if (damageTimers[entity] >= 1f)
            {
                damageTimers[entity] = 0f;
                DealContinuousDamage(entity, skillDamage);

                // 방금 죽었을 수도 있음
                if (entity.IsDead())
                {
                    HandleDeadEntity(entity);
                }
            }
        }
    }

    private void HandleDeadEntity(Entity entity)
    {
        Edible edible = entity.GetComponent<Edible>();
        if (edible != null)
            TryStartSucking(edible);

        entitiesInRange.Remove(entity);
        damageTimers.Remove(entity);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Worm>() != null) return;

        Entity entity = collision.GetComponent<Entity>();
        if (entity != null && !entitiesInRange.Contains(entity))
        {
            entitiesInRange.Add(entity);
            damageTimers[entity] = 0.99f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Entity entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            // 직접 Remove 금지 → 충돌 예방
            pendingRemove.Add(entity);
        }
    }

    private void TryStartSucking(Edible edible)
    {
        if (edible == null) return;
        if (suckingEntities.ContainsKey(edible)) return;

        Coroutine c = StartCoroutine(SuckToMouth(edible));
        suckingEntities[edible] = c;
    }

    private IEnumerator SuckToMouth(Edible edible)
    {
        Transform edibleTf = edible.transform;
        Transform mouth = Worm.Instance.wormHead.transform;

        while (true)
        {
            if (edible == null || mouth == null)
                yield break;

            // 점점 빠르게 가까워지는 느낌의 Lerp
            edibleTf.position = Vector3.Lerp(edibleTf.position, mouth.position, Time.deltaTime * 5f);

            // ★ 거리 체크 → "먹기 처리" + Destroy
            float dist = Vector3.Distance(edibleTf.position, mouth.position);
            if (dist <= 1f)
            {
                Worm.Instance.GainExp(edible.GetExpReward());
                Worm.Instance.RestoreHunger(edible.GetHungerRestore());

                LogHelper.Log($"{edible.GetEdibleName()}을(를) 흡입해서 먹었다!");

                Destroy(edible.gameObject); // ← 실제 파괴 시점

                break;
            }

            yield return null;
        }

        suckingEntities.Remove(edible);
    }


    private void OnDestroy()
    {
        foreach (var c in suckingEntities.Values)
            if (c != null) StopCoroutine(c);
    }
}
