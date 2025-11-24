using UnityEngine;

public class ParasiteWormAI : MonoBehaviour
{
    public Transform owner; // 대지렁이 (Worm.Instance)
    public ParasiteWormDetector detector;

    [Header("AI 이동 설정")]
    public float moveSpeed = 6f;
    public float orbitRadius = 2.2f;
    public float orbitSpeed = 2f;
    public float ownerFollowDistance = 8f;     // 이 거리 이상 멀어지면 owner 따라감

    private float t = 0f;

    private void Start()
    {
        if (owner == null)
            owner = Worm.Instance.transform;

        detector = GetComponentInChildren<ParasiteWormDetector>();
    }

    private void Update()
    {
        // Owner와 거리 체크
        float distToOwner = Vector2.Distance(transform.position, owner.position);
        bool tooFar = distToOwner > ownerFollowDistance;

        // Detector에서 가장 가까운 edible 찾기
        Edible target = tooFar ? null : detector.GetNearestEdible(transform);

        if (target != null)
        {
            OrbitAround(target.transform.position);  // edible 주변에서 ∞ 패턴
        }
        else
        {
            OrbitAround(owner.position);             // owner 주변 ∞ 패턴
        }
    }

    private void OrbitAround(Vector3 center)
    {
        t += Time.deltaTime * orbitSpeed;

        // ∞ 형태(렘니스케이트) 기본형
        float x = Mathf.Sin(t) * orbitRadius;
        float y = Mathf.Sin(t) * Mathf.Cos(t) * orbitRadius;

        Vector3 desiredPos = center + new Vector3(x, y, 0);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            Time.deltaTime * moveSpeed
        );
    }
}
