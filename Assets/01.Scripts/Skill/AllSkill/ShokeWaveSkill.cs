using UnityEngine;

public class ShokeWaveSkill : ActiveSkillBase
{
    [SerializeField] GameObject shokeWavePrefab;

    protected override void Awake()
    {
        shokeWavePrefab = Resources.Load<GameObject>("SkillShockWave");
        currentDamage = baseDamage;
        currentRange = baseRange;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
        worm = Worm.Instance;
    }


    protected override void Execute()
    {
        WormEating wormHead = Worm.Instance.wormHead;

        GameObject spawnedLaser = MonoBehaviour.Instantiate(shokeWavePrefab);
        spawnedLaser.GetComponent<SkillBodyBase>().Init(this);

        spawnedLaser.transform.position = wormHead.transform.position;
    }

    protected override void InitializeBaseStats()
    {
        throw new System.NotImplementedException();
    }
}
