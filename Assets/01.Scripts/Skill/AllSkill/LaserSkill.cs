using UnityEngine;

public class LaserSkill : ActiveSkillBase
{
    [SerializeField] GameObject laserPrefab;

    protected override void Awake()
    {
        laserPrefab = Resources.Load<GameObject>("SkillLaser");
        base.Awake();
    }

    protected override void InitializeBaseStats()
    {
        baseDamage = 50f;
        baseRange = 20f;       // 레이저 길이
        baseCooldown = 5f;
        baseDuration = 0.5f;   // 레이저 지속
    }

    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawned = Instantiate(laserPrefab);
        spawned.GetComponent<SkillBodyBase>().Init(this);

        spawned.transform.position = wormHead.transform.position;
        spawned.transform.up = worm.GetDirection();
        spawned.transform.SetParent(wormHead.transform);

        LogHelper.Log("레이저 스킬 발동!");
    }
}