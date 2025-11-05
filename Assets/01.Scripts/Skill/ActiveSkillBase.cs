using UnityEngine;

public abstract class ActiveSkillBase : MonoBehaviour
{
    public SkillData skillData;

    protected Worm worm;
    protected WormStatus status;

    // 기본 수치
    [SerializeField] protected float baseDamage = 100f;
    [SerializeField] protected float baseRange = 5f;
    [SerializeField] protected float baseCooldown = 5f;
    [SerializeField] protected float baseDuration = 3f;

    // 현재 수치 (레벨 + 패시브 반영)
    public float currentDamage { get; protected set; }
    public float currentRange { get; protected set; }
    public float currentCooldown { get; protected set; }
    public float currentDuration { get; protected set; }

    protected float cooldownTimer = 0f;

    protected virtual void Awake()
    {
        worm = Worm.Instance;
        status = worm.GetComponent<WormStatus>();
        InitializeBaseStats();
        CalculateCurrentStats();
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            Execute();
            cooldownTimer += currentCooldown;
        }
    }

    // ⭐ 각 스킬이 구현해야 함
    protected abstract void Execute();

    // ⭐ 기본 스탯 설정 (각 스킬이 오버라이드)
    protected abstract void InitializeBaseStats();

    // ⭐ 레벨업 시 호출
    public virtual void OnLevelUp(int newLevel)
    {
        skillData.currentLevel = newLevel;
        CalculateCurrentStats();
    }

    // ⭐ 현재 스탯 계산 (레벨 + 패시브)
    protected virtual void CalculateCurrentStats()
    {
        // 레벨에 따른 기본 증가
        float levelMultiplier = 1f + (skillData.currentLevel * skillData.upgradeRate);

        currentDamage = baseDamage * levelMultiplier;
        currentRange = baseRange * levelMultiplier;
        currentCooldown = baseCooldown * levelMultiplier;
        currentDuration = baseDuration * levelMultiplier;

        // 패시브 보너스 반영
        ApplyPassiveBonus();
    }

    // ⭐ 패시브 보너스 반영
    protected virtual void ApplyPassiveBonus()
    {
        if (status == null) return;

        currentDamage *= status.GetAttackMultiplier();
        currentRange *= status.GetRangeMultiplier();
        currentCooldown *= status.GetCooldownMultiplier();
        currentDuration *= status.GetDurationMultiplier();
    }

    // ⭐ 패시브 변경 시 재계산
    public void OnPassiveChanged()
    {
        CalculateCurrentStats();
    }
}