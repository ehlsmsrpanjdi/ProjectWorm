using UnityEngine;

public class VaccumSkill : ActiveSkillBase
{
    [SerializeField] GameObject vaccumPrefab;

    protected override void Awake()
    {
        vaccumPrefab = Resources.Load<GameObject>("SkillVaccum");
        base.Awake(); // ⭐ 추가!
    }

    protected override void InitializeBaseStats()
    {
        baseDamage = 15f;      // 초당 데미지
        baseRange = 8f;        // 범위
        baseCooldown = 5f;     // 쿨타임
        baseDuration = 4f;     // 지속 시간
    }

    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawned = Instantiate(vaccumPrefab);
        spawned.GetComponent<SkillBodyBase>().Init(this);

        spawned.transform.position = wormHead.transform.position;
        spawned.transform.up = worm.GetDirection();
        spawned.transform.SetParent(wormHead.transform);

        LogHelper.Log("흡입 스킬 발동!");
    }
}