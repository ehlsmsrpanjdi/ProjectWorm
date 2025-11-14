using UnityEngine;

public class VaccumSkill : ActiveSkillBase
{
    [SerializeField] GameObject vaccumPrefab;

    protected override void Awake()
    {
        vaccumPrefab = Resources.Load<GameObject>("SkillVaccum");
        currentDamage = baseDamage;
        currentRange = baseRange;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
        worm = Worm.Instance;
    }

    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawnedLaser = MonoBehaviour.Instantiate(vaccumPrefab);
        spawnedLaser.GetComponent<SkillBodyBase>().Init(this);

        spawnedLaser.transform.position = wormHead.transform.position;

        spawnedLaser.transform.up = worm.GetDirection();

        spawnedLaser.transform.SetParent(Worm.Instance.wormHead.transform);
    }

    protected override void InitializeBaseStats()
    {
        throw new System.NotImplementedException();
    }
}
