using UnityEngine;

public class ChaseWormAI : MonoBehaviour
{
    [Header("참조")]
    private ChaseWormDetector detector;
    private Transform wormTransform;

    [Header("8자 운동 설정")]
    public float orbitRadius = 3f;
    public float orbitSpeed = 2f;
    public float moveSpeed = 5f;
    private float angle = 0f;

    [Header("복귀 설정")]
    public float returnSpeed = 10f;
    public float maxDistanceFromWorm = 30f;
    public float attackCooldown = 3f; // ⭐ 복귀 후 쿨다운 (초)

    private enum State
    {
        OrbitWorm,
        AttackEdible,
        ReturnToWorm,
        Cooldown // ⭐ 새로운 상태: 쿨다운 중
    }

    private State currentState = State.OrbitWorm;
    private Transform targetEdible = null;

    // ⭐ 쿨다운 타이머
    private float cooldownTimer = 0f;

    private void Awake()
    {
        detector = GetComponent<ChaseWormDetector>();
        if (detector == null)
        {
            LogHelper.LogError("ChaseWormDetector를 찾을 수 없습니다!");
        }
    }

    private void Start()
    {
        if (Worm.Instance != null)
        {
            wormTransform = Worm.Instance.wormHead.transform;
        }
    }

    private void Update()
    {
        if (wormTransform == null) return;

        UpdateState();

        switch (currentState)
        {
            case State.OrbitWorm:
                OrbitAroundTarget(wormTransform.position);
                break;

            case State.AttackEdible:
                if (IsTargetValid())
                {
                    OrbitAroundTarget(targetEdible.position);
                }
                else
                {
                    targetEdible = null;
                    currentState = State.OrbitWorm;
                }
                break;

            case State.ReturnToWorm:
                ReturnToWorm();
                break;

            case State.Cooldown: // ⭐ 쿨다운 상태
                OrbitAroundTarget(wormTransform.position); // 플레이어 주변 8자
                cooldownTimer -= Time.deltaTime;

                if (cooldownTimer <= 0f)
                {
                    currentState = State.OrbitWorm;
                    LogHelper.Log("ParasiteWorm: 쿨다운 종료, 공격 가능!");
                }
                break;
        }
    }

    private bool IsTargetValid()
    {
        if (targetEdible == null) return false;

        try
        {
            if (targetEdible.gameObject == null) return false;

            Edible edible = targetEdible.GetComponent<Edible>();
            if (edible == null || edible.IsDead()) return false;

            return true;
        }
        catch (MissingReferenceException)
        {
            return false;
        }
    }

    private void UpdateState()
    {
        float distanceToWorm = Vector2.Distance(transform.position, wormTransform.position);

        switch (currentState)
        {
            case State.OrbitWorm:
                // ⭐ OrbitWorm일 때만 몬스터 감지
                Edible nearestEdible = detector.GetNearestEdible(transform);
                if (nearestEdible != null)
                {
                    targetEdible = nearestEdible.transform;
                    currentState = State.AttackEdible;
                    LogHelper.Log($"ParasiteWorm: AttackEdible 전환 → {nearestEdible.name}");
                }

                if (distanceToWorm > maxDistanceFromWorm)
                {
                    currentState = State.ReturnToWorm;
                    LogHelper.Log("ParasiteWorm: ReturnToWorm 전환");
                }
                break;

            case State.AttackEdible:
                if (!IsTargetValid())
                {
                    LogHelper.Log("ParasiteWorm: 타겟 무효, 새 타겟 검색");

                    Edible newTarget = detector.GetNearestEdible(transform);
                    if (newTarget != null)
                    {
                        targetEdible = newTarget.transform;
                        LogHelper.Log($"ParasiteWorm: 새 타겟 → {newTarget.name}");
                    }
                    else
                    {
                        targetEdible = null;
                        currentState = State.OrbitWorm;
                        LogHelper.Log("ParasiteWorm: OrbitWorm 전환");
                    }
                }

                if (distanceToWorm > maxDistanceFromWorm)
                {
                    targetEdible = null;
                    currentState = State.ReturnToWorm;
                    LogHelper.Log("ParasiteWorm: ReturnToWorm 전환 (너무 멀어짐)");
                }
                break;

            case State.ReturnToWorm:
                // ⭐ 복귀 완료 → 쿨다운 시작!
                if (distanceToWorm < orbitRadius * 2f)
                {
                    cooldownTimer = attackCooldown;
                    currentState = State.Cooldown;
                    LogHelper.Log($"ParasiteWorm: Cooldown 시작 ({attackCooldown}초)");
                }
                break;

            case State.Cooldown:
                // ⭐ 쿨다운 중에는 몬스터 무시
                // Update()에서 타이머 감소 처리

                // 쿨다운 중에도 너무 멀어지면 복귀
                if (distanceToWorm > maxDistanceFromWorm)
                {
                    currentState = State.ReturnToWorm;
                    LogHelper.Log("ParasiteWorm: 쿨다운 중 복귀");
                }
                break;
        }
    }

    private void OrbitAroundTarget(Vector3 centerPos)
    {
        angle += orbitSpeed * Time.deltaTime;
        if (angle > Mathf.PI * 2f)
        {
            angle -= Mathf.PI * 2f;
        }

        float x = orbitRadius * Mathf.Sin(angle);
        float y = orbitRadius * Mathf.Sin(angle) * Mathf.Cos(angle);

        Vector3 targetPosition = centerPos + new Vector3(x, y, 0f);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );
    }

    private void ReturnToWorm()
    {
        Vector3 directionToWorm = (wormTransform.position - transform.position).normalized;
        transform.position += directionToWorm * returnSpeed * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // ⭐ 상태별 색상 (Cooldown 추가)
        Gizmos.color = currentState switch
        {
            State.OrbitWorm => Color.green,
            State.AttackEdible => Color.red,
            State.ReturnToWorm => Color.yellow,
            State.Cooldown => Color.blue, // ⭐ 파란색 = 쿨다운
            _ => Color.white
        };

        Gizmos.DrawWireSphere(transform.position, 1f);

        // ⭐ 쿨다운 타이머 표시
#if UNITY_EDITOR
        if (currentState == State.Cooldown)
        {
            UnityEditor.Handles.Label(
                transform.position + Vector3.up * 2f,
                $"쿨다운: {cooldownTimer:F1}초"
            );
        }
#endif

        // 복귀 범위
        if (wormTransform != null)
        {
            Gizmos.color = new Color(1, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(wormTransform.position, maxDistanceFromWorm);

            float distance = Vector2.Distance(transform.position, wormTransform.position);
            Gizmos.color = distance > maxDistanceFromWorm ? Color.red : Color.cyan;
            Gizmos.DrawLine(transform.position, wormTransform.position);
        }

        // 타겟 표시
        if (currentState == State.AttackEdible && IsTargetValid())
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetEdible.position);
            Gizmos.DrawWireSphere(targetEdible.position, 2f);
        }
    }
}