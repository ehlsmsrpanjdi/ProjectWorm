using UnityEngine;

public class Tank : Edible
{
    [Header("이동")]
    public float moveSpeed = 2f;
    public float stopDistance = 20f; // 멈추는 거리

    [Header("공격")]
    [SerializeField] GameObject bulletObj;
    public float fireInterval = 1f; // 발사 간격
    private float fireTimer = 0f;

    private float distanceToWorm = 0f;
    private bool isWormAboveGround = false;

    protected override void Reset()
    {
        base.Reset();
        bulletObj = Resources.Load<GameObject>("Bullet/TestBullet");
    }

    private void Update()
    {
        if (Worm.Instance == null || Worm.Instance.wormHead == null) return;

        // ⭐ Worm이 지상(y >= 0)에 있는지 체크
        isWormAboveGround = Worm.Instance.wormHead.transform.position.y >= 0f;

        // ⭐ 지상에 있을 때만 동작
        if (isWormAboveGround)
        {
            MoveFunction();
            FireFunction();
        }
        else
        {
            // 땅속에 있으면 정지
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void MoveFunction()
    {
        Vector3 wormPosition = Worm.Instance.wormHead.transform.position;

        // ⭐ Worm까지의 거리 계산 (X축만)
        float deltaX = wormPosition.x - transform.position.x;
        distanceToWorm = Mathf.Abs(deltaX);

        // ⭐ 거리가 5 이상이면 다가가기
        if (distanceToWorm > stopDistance)
        {
            float moveDirection = Mathf.Sign(deltaX); // -1 또는 1
            rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            // ⭐ 거리가 5 이하면 정지
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void FireFunction()
    {
        // ⭐ 거리가 5 이하일 때만 공격
        if (distanceToWorm <= stopDistance)
        {
            fireTimer += Time.deltaTime;

            if (fireTimer >= fireInterval)
            {
                Fire();
                fireTimer = 0f;
            }
        }
        else
        {
            // 사거리 밖이면 타이머 리셋
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

        LogHelper.Log("Soldier 발사!");
    }
}