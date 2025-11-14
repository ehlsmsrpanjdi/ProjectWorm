using UnityEngine;

public class AirCannonSkill : ActiveSkillBase
{
    [SerializeField] GameObject airCannonPrefab;

    protected override void Awake()
    {
        airCannonPrefab = Resources.Load<GameObject>("SkillAirCannon");
        base.Awake();
    }

    protected override void InitializeBaseStats()
    {
        baseDamage = 40f;
        baseRange = 1f;        // 투사체 크기
        baseCooldown = 5f;
        baseDuration = 5f;     // 투사체 생존 시간
    }

    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawned = Instantiate(airCannonPrefab);
        spawned.GetComponent<SkillBodyBase>().Init(this);

        spawned.transform.position = wormHead.transform.position;

        LogHelper.Log("진공파 스킬 발동!");
    }
}