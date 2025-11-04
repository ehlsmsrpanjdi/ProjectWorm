using UnityEngine;

// ⭐ 추가: 먹을 수 없는 객체 클래스 (폭탄, 가시 등)
public class Inedible : Entity
{
    [Header("위험 정보")]
    [SerializeField] private float damageToWorm = 50f; // 지렁이에게 주는 데미지
    [SerializeField] private bool canBecomeEdible = true; // 능력으로 먹을 수 있게 될지

    // ⭐ 추가: 지렁이와 충돌 시
    public void OnTouchWorm()
    {
        // 지렁이에게 데미지
        Worm.Instance?.TakeDamage(damageToWorm);

        LogHelper.Log($"{gameObject.name}이(가) 지렁이에게 {damageToWorm} 데미지를 줌!");
    }

    // ⭐ 추가: 특수 능력으로 먹을 수 있게 변경
    public void ConvertToEdible(int expReward, float hungerRestore)
    {
        if (!canBecomeEdible) return;

        // Edible 컴포넌트로 교체
        Edible edibleComponent = gameObject.AddComponent<Edible>();

        // 정보 설정은 Inspector에서 직접 해야 함
        LogHelper.Log($"{gameObject.name}이(가) 먹을 수 있게 변경됨!");

        Destroy(this); // 자기 자신(Inedible) 제거
    }

    // ⭐ 추가: Getter
    public float GetDamageToWorm() => damageToWorm;
    public bool CanBecomeEdible() => canBecomeEdible;
}