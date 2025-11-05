using UnityEngine;

public class LaserSkill : ActiveSkillBase
{
    [SerializeField] GameObject laserPrefab;

    protected override void Awake()
    {
        laserPrefab = Resources.Load<GameObject>("SkillLaser");
        currentDamage = baseDamage;
        currentRange = baseRange;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
        worm = Worm.Instance;
    }

    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawnedLaser = MonoBehaviour.Instantiate(laserPrefab);
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
