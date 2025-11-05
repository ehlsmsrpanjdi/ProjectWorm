using UnityEngine;

public class AirCannonSkill : ActiveSkillBase
{
    [SerializeField] GameObject airCannonPrefab;

    protected override void Awake()
    {
        airCannonPrefab = Resources.Load<GameObject>("SkillAirCannon");
        currentDamage = baseDamage;
        currentRange = baseRange;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
        worm = Worm.Instance;
    }


    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawnedLaser = MonoBehaviour.Instantiate(airCannonPrefab);
        spawnedLaser.GetComponent<SkillBodyBase>().Init(this);

        spawnedLaser.transform.position = wormHead.transform.position;
    }

    protected override void InitializeBaseStats()
    {
        throw new System.NotImplementedException();
    }
}
