using UnityEngine;

public class VirusSkill : ActiveSkillBase
{
    [SerializeField] GameObject virusPrefab;

    protected override void Awake()
    {
        virusPrefab = Resources.Load<GameObject>("SkillVirus");
        currentDamage = baseDamage;
        currentRange = baseRange;
        currentCooldown = baseCooldown;
        currentDuration = baseDuration;
        worm = Worm.Instance;
    }


    protected override void Execute()
    {
        WormTale wormTale = Worm.Instance.wormTale;

        GameObject spawnedLaser = MonoBehaviour.Instantiate(virusPrefab);
        spawnedLaser.GetComponent<SkillBodyBase>().Init(this);

        spawnedLaser.transform.position = wormTale.transform.position;
    }

    protected override void InitializeBaseStats()
    {
        throw new System.NotImplementedException();
    }
}
