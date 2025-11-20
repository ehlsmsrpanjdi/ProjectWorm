using UnityEngine;

public class Helicopter : Edible
{
    [Header("이동")]
    public float maxMoveSpeed = 5f; // 최대 속도
    public float acceleration = 5f; // 가속도
    public float patrolDistance = 30f; // 순찰 거리 (플레이어 지하 시)
    public float combatDistance = 10f; // 전투 거리 (플레이어 지상 시)

    [Header("공격")]
    [SerializeField] GameObject bulletObj;
    public float fireInterval = 1f;
    private float fireTimer = 0f;

    private bool isWormAboveGround = false;
    private float currentSpeed = 0f; // 현재 속도
    private float targetX = 0f; // 목표 X 위치
    private bool movingRight = true; // 오른쪽으로 이동 중?

    protected override void Reset()
    {
        base.Reset();
        bulletObj = Resources.Load<GameObject>("Bullet/TestBullet");
    }

    private void Start()
    {
        // 초기 목표 설정
        UpdateTargetPosition();
    }

    private void Update()
    {
        if (Worm.Instance == null || Worm.Instance.wormHead == null) return;

        // Worm이 지상에 있는지 체크
        isWormAboveGround = Worm.Instance.wormHead.transform.position.y >= 0f;

        // 이동
        MoveFunction();

        // 지상일 때만 공격
        if (isWormAboveGround)
        {
            FireFunction();
        }
    }

    private void MoveFunction()
    {
        Vector3 wormPosition = Worm.Instance.wormHead.transform.position;

        // ⭐ 현재 사용할 거리 (지상/지하에 따라)
        float activeDistance = isWormAboveGround ? combatDistance : patrolDistance;

        // ⭐ 목표 위치 도달했는지 체크
        float distanceToTarget = Mathf.Abs(transform.position.x - targetX);

        if (distanceToTarget < 1f)
        {
            // 목표 도달 → 반대편으로 전환
            movingRight = !movingRight;
            UpdateTargetPosition();
        }

        // ⭐ 목표 위치로 이동 (가속도 기반)
        float directionToTarget = Mathf.Sign(targetX - transform.position.x);

        // 목표와의 거리에 비례한 가속도 (멀수록 빠르게)
        float distanceFactor = Mathf.Clamp01(distanceToTarget / activeDistance);
        float targetSpeed = maxMoveSpeed * distanceFactor;

        // ⭐ 부드럽게 속도 변경 (가속/감속)
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed * directionToTarget, acceleration * Time.deltaTime);

        // 속도 적용
        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);
    }

    // ⭐ 목표 위치 업데이트
    private void UpdateTargetPosition()
    {
        if (Worm.Instance == null || Worm.Instance.wormHead == null) return;

        Vector3 wormPosition = Worm.Instance.wormHead.transform.position;
        float activeDistance = isWormAboveGround ? combatDistance : patrolDistance;

        // 좌우 왕복
        if (movingRight)
        {
            targetX = wormPosition.x + activeDistance;
        }
        else
        {
            targetX = wormPosition.x - activeDistance;
        }

        LogHelper.Log($"헬리콥터 목표 변경: {(movingRight ? "오른쪽" : "왼쪽")} → X={targetX}");
    }

    private void FireFunction()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    private void Fire()
    {
        if (bulletObj == null)
        {
            LogHelper.LogError("Bullet prefab이 없습니다!");
            return;
        }

        Vector3 wormPosition = Worm.Instance.wormHead.transform.position;
        Vector3 direction = (wormPosition - transform.position).normalized;

        GameObject spawnedBullet = Instantiate(bulletObj);
        spawnedBullet.transform.position = transform.position;

        BulletBase bullet = spawnedBullet.GetComponent<BulletBase>();
        if (bullet != null)
        {
            bullet.Init(direction);
        }
    }

    // ⭐ 디버그용
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || Worm.Instance == null) return;

        Vector3 wormPosition = Worm.Instance.wormHead.transform.position;
        float activeDistance = isWormAboveGround ? combatDistance : patrolDistance;

        // 순찰 범위 표시
        Gizmos.color = Color.cyan;
        Vector3 leftPoint = new Vector3(wormPosition.x - activeDistance, wormPosition.y, 0);
        Vector3 rightPoint = new Vector3(wormPosition.x + activeDistance, wormPosition.y, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);

        // 목표 위치 표시
        Gizmos.color = Color.yellow;
        Vector3 targetPos = new Vector3(targetX, transform.position.y, 0);
        Gizmos.DrawWireSphere(targetPos, 1f);

        // 이동 방향 화살표
        Gizmos.color = Color.green;
        Vector3 velocityDir = new Vector3(currentSpeed, 0, 0).normalized;
        Gizmos.DrawRay(transform.position, velocityDir * 2f);
    }
}
