using UnityEngine;

public class SandstormSkill : ActiveSkillBase
{
    [SerializeField] GameObject sandstormPrefab;

    // 생성 관련
    private float spawnTimer = 0f;
    private float spawnInterval = 2f; // 생성 간격
    private int spawnCount = 1; // 한 번에 생성할 개수

    protected override void Awake()
    {
        sandstormPrefab = Resources.Load<GameObject>("SkillSandstorm");
        base.Awake();
    }

    protected override void InitializeBaseStats()
    {
        baseDamage = 20f;      // 초당 데미지
        baseRange = 15f;       // 생성 반경
        baseCooldown = 2f;     // 생성 간격 (기본 2초)
        baseDuration = 3f;     // 모래 소용돌이 지속 시간
    }

    protected override void CalculateCurrentStats()
    {
        base.CalculateCurrentStats();

        // ⭐ 생성 간격 (레벨 오를수록 감소)
        spawnInterval = currentCooldown;

        // ⭐ 생성 개수 (2레벨마다 +1)
        // 1강: 1개, 2강: 2개, 3강: 2개, 4강: 3개, 5강: 3개
        spawnCount = 1 + (skillData.currentLevel / 2);

        LogHelper.Log($"모래 소용돌이: 간격={spawnInterval}초, 개수={spawnCount}개");
    }

    protected override void Update()
    {
        if (!isInitialized) return;

        // ⭐ 일반 Update가 아니라 커스텀 타이머
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            Execute();
            spawnTimer = spawnInterval;
        }
    }

    protected override void Execute()
    {
        Vector3 wormPos = worm.transform.position;

        // ⭐ 지정된 개수만큼 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 랜덤 위치 계산 (worm 주변 currentRange 반경 내)
            Vector2 randomOffset = Random.insideUnitCircle * currentRange;
            Vector3 spawnPos = wormPos + new Vector3(randomOffset.x, randomOffset.y, 0);

            // 생성
            GameObject spawned = Instantiate(sandstormPrefab);
            spawned.GetComponent<SkillBodyBase>().Init(this);
            spawned.transform.position = spawnPos;

            LogHelper.Log($"모래 소용돌이 생성 ({i + 1}/{spawnCount})");
        }
    }
}