using UnityEngine;

public class ShokeWaveSkill : ActiveSkillBase
{
    [SerializeField] GameObject shokeWavePrefab;

    protected override void Awake()
    {
        shokeWavePrefab = Resources.Load<GameObject>("SkillShockWave");
        base.Awake();
    }

    protected override void InitializeBaseStats()
    {
        baseDamage = 100f;     // 즉사급
        baseRange = 15f;
        baseCooldown = 5f;
        baseDuration = 1f;
    }

    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawned = Instantiate(shokeWavePrefab);
        spawned.GetComponent<SkillBodyBase>().Init(this);

        spawned.transform.position = wormHead.transform.position;

        LogHelper.Log("충격파 스킬 발동!");
    }
}