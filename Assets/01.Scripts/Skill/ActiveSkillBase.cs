using UnityEngine;

public abstract class ActiveSkillBase : MonoBehaviour
{
    public SkillData skillData;

    protected Worm worm;
    protected WormStatus status;

    [SerializeField] protected float baseDamage = 100f;
    [SerializeField] protected float baseRange = 5f;
    [SerializeField] protected float baseCooldown = 5f;
    [SerializeField] protected float baseDuration = 3f;

    public float currentDamage { get; protected set; }
    public float currentRange { get; protected set; }
    public float currentCooldown { get; protected set; }
    public float currentDuration { get; protected set; }

    protected float cooldownTimer = 0f;

    // ⭐ 초기화 여부 플래그
    protected bool isInitialized = false;

    // ⭐ Awake는 최소한만!
    protected virtual void Awake()
    {
        worm = Worm.Instance;
        status = worm.status;
    }

    // ⭐ 외부에서 명시적으로 호출하는 초기화
    public void Initialize(SkillData data)
    {
        if (isInitialized)
        {
            LogHelper.LogWarrning($"{GetType().Name} 이미 초기화됨");
            return;
        }

        skillData = data;

        // 기본 스탯 설정
        InitializeBaseStats();

        // 현재 스탯 계산
        CalculateCurrentStats();

        isInitialized = true;

        LogHelper.Log($"{skillData.skillName} 초기화 완료: 데미지={currentDamage}, 범위={currentRange}, 쿨타임={currentCooldown}");
    }

    protected virtual void Update()
    {
        if (!isInitialized) return; // ⭐ 초기화 전엔 실행 안 함

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f)
        {
            Execute();
            cooldownTimer = currentCooldown;
        }
    }

    protected abstract void Execute();
    protected abstract void InitializeBaseStats();

    public virtual void OnLevelUp(int newLevel)
    {
        skillData.currentLevel = newLevel;
        CalculateCurrentStats();
    }

    protected virtual void CalculateCurrentStats()
    {
        if (skillData == null || status == null)
        {
            LogHelper.LogError("skillData 또는 status가 null!");
            return;
        }

        // 레벨에 따른 기본 증가
        float levelMultiplier = 1f + (skillData.currentLevel * skillData.upgradeRate);

        currentDamage = baseDamage * levelMultiplier;
        currentRange = baseRange * levelMultiplier;
        currentCooldown = baseCooldown * levelMultiplier;
        currentDuration = baseDuration * levelMultiplier;

        // 패시브 보너스 반영
        ApplyPassiveBonus();
    }

    protected virtual void ApplyPassiveBonus()
    {
        if (status == null) return;

        currentDamage *= status.GetAttackMultiplier();
        currentRange *= status.GetRangeMultiplier();
        currentCooldown *= status.GetCooldownMultiplier();
        currentDuration *= status.GetDurationMultiplier();
    }

    public void OnPassiveChanged()
    {
        CalculateCurrentStats();
    }
}