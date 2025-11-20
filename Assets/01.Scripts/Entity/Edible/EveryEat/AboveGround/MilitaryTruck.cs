using UnityEngine;

public class MilitaryTruck : Edible
{
    [Header("이동")]
    public float moveSpeed = 10f;
    public float deployDistance = 8f; // 배치 시작 거리

    [Header("군인 스폰")]
    public GameObject soldierPrefab;
    public int maxSoldiers = 6;
    public float spawnInterval = 1f; // 초당 1마리
    public float spawnOffset = 1f; // 트럭으로부터 떨어진 거리

    private enum TruckState
    {
        Approach,  // 플레이어 향해 접근
        Deploy,    // 정지 & 군인 배치
        Retreat    // 반대 방향으로 후퇴
    }

    private TruckState currentState = TruckState.Approach;
    private float distanceToWorm = 0f;
    private bool isWormAboveGround = false;

    // Deploy 상태 변수
    private int spawnedCount = 0;
    private float spawnTimer = 0f;

    // Retreat 방향
    private float retreatDirection = 0f;

    protected override void Reset()
    {
        base.Reset();
        soldierPrefab = Resources.Load<GameObject>("Enemy/Soldier");
    }


    private void Update()
    {
        if (Worm.Instance == null || Worm.Instance.wormHead == null) return;

        // Worm이 지상에 있는지 체크
        isWormAboveGround = Worm.Instance.wormHead.transform.position.y >= 0f;

        // 거리 계산
        Vector3 wormPosition = Worm.Instance.wormHead.transform.position;
        float deltaX = wormPosition.x - transform.position.x;
        distanceToWorm = Mathf.Abs(deltaX);

        // 상태 머신
        switch (currentState)
        {
            case TruckState.Approach:
                UpdateApproach(deltaX);
                break;

            case TruckState.Deploy:
                UpdateDeploy();
                break;

            case TruckState.Retreat:
                UpdateRetreat();
                break;
        }
    }

    // ⭐ 상태 1: 접근
    private void UpdateApproach(float deltaX)
    {
        // 플레이어가 지하에 있으면 기존 방향 유지
        if (!isWormAboveGround)
        {
            // 마지막 이동 방향 유지
            if (rb.linearVelocity.x != 0)
            {
                float currentDirection = Mathf.Sign(rb.linearVelocity.x);
                rb.linearVelocity = new Vector2(currentDirection * moveSpeed, rb.linearVelocity.y);
            }
            return;
        }

        // 플레이어가 8칸 이내면 Deploy 상태로 전환
        if (distanceToWorm <= deployDistance)
        {
            ChangeState(TruckState.Deploy);
            return;
        }

        // 플레이어 향해 이동
        float moveDirection = Mathf.Sign(deltaX);
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
    }

    // ⭐ 상태 2: 배치
    private void UpdateDeploy()
    {
        // 정지
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        // 군인 스폰
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && spawnedCount < maxSoldiers)
        {
            SpawnSoldier();
            spawnedCount++;
            spawnTimer = 0f;

            LogHelper.Log($"군인 스폰: {spawnedCount}/{maxSoldiers}");

            // 6마리 다 스폰했으면 후퇴
            if (spawnedCount >= maxSoldiers)
            {
                ChangeState(TruckState.Retreat);
            }
        }
    }

    // ⭐ 상태 3: 후퇴
    private void UpdateRetreat()
    {
        // 플레이어 반대 방향으로 이동
        rb.linearVelocity = new Vector2(retreatDirection * moveSpeed, rb.linearVelocity.y);
    }

    // ⭐ 군인 스폰
    private void SpawnSoldier()
    {
        if (soldierPrefab == null)
        {
            LogHelper.LogError("Soldier prefab이 없습니다!");
            return;
        }

        // 트럭 뒤쪽에서 스폰
        Vector3 spawnPosition = transform.position + new Vector3(-retreatDirection * spawnOffset, 0f, 0f);

        GameObject soldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);

        LogHelper.Log($"군인 스폰 완료: {soldier.name}");
    }

    // ⭐ 상태 전환
    private void ChangeState(TruckState newState)
    {
        LogHelper.Log($"MilitaryTruck 상태 전환: {currentState} → {newState}");

        currentState = newState;

        switch (newState)
        {
            case TruckState.Deploy:
                // Deploy 시작 시 초기화
                spawnedCount = 0;
                spawnTimer = 0f;

                // 후퇴 방향 결정 (현재 이동 방향의 반대)
                retreatDirection = -Mathf.Sign(rb.linearVelocity.x);
                if (retreatDirection == 0f) retreatDirection = -1f; // 기본값

                break;

            case TruckState.Retreat:
                // 후퇴 시작
                LogHelper.Log("배치 완료! 후퇴 시작");
                break;
        }
    }

    // ⭐ 디버그용
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 배치 범위 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, deployDistance);

        // 현재 상태 표시
        Vector3 textPos = transform.position + Vector3.up * 2f;
        UnityEditor.Handles.Label(textPos, $"State: {currentState}\nSoldiers: {spawnedCount}/{maxSoldiers}");
    }
}
