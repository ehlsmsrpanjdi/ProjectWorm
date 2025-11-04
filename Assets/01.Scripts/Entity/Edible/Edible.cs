using UnityEngine;

// ⭐ 추가: 먹을 수 있는 객체 클래스
public class Edible : Entity
{
    [Header("먹이 정보")]
    [SerializeField] private string edibleName = "작은 벌레";
    [SerializeField] private int expReward = 10;
    [SerializeField] private float hungerRestore = 10f;

    [Header("먹기 이펙트")]
    [SerializeField] private GameObject eatEffect;

    // ⭐ 추가: 먹혔을 때 처리
    public virtual void OnEaten()
    {
        if (isDead) return;
        isDead = true;

        LogHelper.Log($"{edibleName}을(를) 먹었다!");

        // 먹기 이펙트
        if (eatEffect != null)
        {
            Instantiate(eatEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // ⭐ 추가: Getter 메서드들
    public string GetEdibleName() => edibleName;
    public int GetExpReward() => expReward;
    public float GetHungerRestore() => hungerRestore;
}