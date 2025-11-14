using UnityEngine;

public class VirusSkill : ActiveSkillBase
{
    [SerializeField] GameObject virusPrefab;

    protected override void Awake()
    {
        virusPrefab = Resources.Load<GameObject>("SkillVirus");
        base.Awake();
    }

    protected override void InitializeBaseStats()
    {
        baseDamage = 5f;
        baseRange = 1f;
        baseCooldown = 5f;
        baseDuration = 3f;
    }

    protected override void Execute()
    {
        WormTale wormTale = Worm.Instance.wormTale;

        GameObject spawned = Instantiate(virusPrefab);
        spawned.GetComponent<SkillBodyBase>().Init(this);

        spawned.transform.position = wormTale.transform.position;

        LogHelper.Log("염산 스킬 발동!");
    }
}