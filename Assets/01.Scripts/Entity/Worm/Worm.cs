using System;
using UnityEngine;

// ⭐ 추가: Entity 상속
public class Worm : Entity
{
    static Worm instance;
    public static Worm Instance
    {
        get { return instance; }
    }

    WormStatus status = new WormStatus();

    [Header("지렁이 전투 스탯")]
    public float biteDamage = 50f;  // ⭐ 추가: 물기 데미지

    // ⭐ 추가: Entity의 Awake 오버라이드
    protected override void Awake()
    {
        base.Awake(); // Entity의 Awake 호출
        instance = this;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        ComponentUpdate(deltaTime);
    }

    void ComponentUpdate(float _DeltaTime)
    {
        bool isAlive = status.Update(_DeltaTime);

        if (!isAlive)
        {
            // 배고파서 죽음
            Die();
        }
    }

    // ⭐ 추가: 죽음 처리 오버라이드
    protected override void Die()
    {
        LogHelper.Log("지렁이가 죽었다!");
        // TODO: 게임오버 UI 표시
        base.Die();
    }

    #region "StatusChange"

    public void GainExp(float _Exp)
    {
        status.GainExp(_Exp);
    }

    public void RestoreHunger(float _Amount)
    {
        status.RestoreHunger(_Amount);
    }

    // ⭐ 추가: 먹을 수 있는 객체 먹기
    public bool TryEat(Edible edible)
    {
        if (edible == null || edible.IsDead()) return false;

        float targetMaxHealth = edible.GetMaxHealth();

        // 물기 데미지가 상대 최대체력보다 높으면 즉시 먹기
        if (biteDamage >= targetMaxHealth)
        {
            // 한 번에 먹기 성공
            GainExp(edible.GetExpReward());
            RestoreHunger(edible.GetHungerRestore());
            edible.OnEaten();

            LogHelper.Log($"{edible.GetEdibleName()}을(를) 한입에 먹었다!");
            return true;
        }
        else
        {
            // 체력만 깎기
            edible.TakeDamage(biteDamage);
            LogHelper.Log($"{edible.GetEdibleName()}을(를) 물었다! (데미지: {biteDamage})");

            // 죽었으면 먹기
            if (edible.IsDead())
            {
                GainExp(edible.GetExpReward());
                RestoreHunger(edible.GetHungerRestore());
            }

            return false;
        }
    }

    // ⭐ 추가: 먹을 수 없는 객체와 충돌
    public void TouchInedible(Inedible inedible)
    {
        if (inedible == null || inedible.IsDead()) return;

        // 지렁이가 데미지 받음
        inedible.OnTouchWorm();

        // TODO: 특수 능력이 있다면 먹을 수 있게 변환
        // if (HasSpecialAbility("EatEverything"))
        // {
        //     inedible.ConvertToEdible(50, 30f);
        // }
    }

    #endregion

    #region "UI"

    public void SetHungerUIBind(Action<float> _Action)
    {
        status.OnHungryChange += _Action;
    }

    public void SetExpUIBind(Action<float> _Action)
    {
        status.OnExpchange += _Action;
    }

    public void SetLevelUpUIBind(Action<int> _Action)
    {
        status.OnLevelUp += _Action;
    }

    #endregion

    #region "Debug"

    public void DebugGainExp(float _Exp)
    {
        GainExp(_Exp);
    }

    // ⭐ 추가: 디버그용 데미지 테스트
    public void DebugTakeDamage(float _Damage)
    {
        TakeDamage(_Damage);
    }

    #endregion

    private void OnDestroy()
    {
        instance = null;
    }
}