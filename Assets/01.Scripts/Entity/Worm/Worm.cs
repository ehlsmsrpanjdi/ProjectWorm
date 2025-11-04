using System;
using UnityEngine;

// ⭐ 수정: Entity 상속하지만 health는 사용 안 함 (Hunger를 체력으로 사용)
public class Worm : Entity
{
    static Worm instance;
    public static Worm Instance
    {
        get { return instance; }
    }

    WormStatus status = new WormStatus();
    private WormAnimation wormAnimation;
    private WormNavi wormNavi;

    [Header("지렁이 전투 스탯")]
    public float biteDamage = 50f;

    public Vector3 OriginScale { get; private set; }



    protected override void Awake()
    {
        instance = this;

        wormAnimation = GetComponentInChildren<WormAnimation>();
        wormNavi = GetComponentInChildren<WormNavi>();

        OriginScale = transform.localScale;

        status.SetScaleRatio(OriginScale.x);
        OnSizeChanged(OriginScale.x);

        status.OnSizeChange += OnSizeChanged;
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
            Die();
        }
    }

    // ⭐ 수정: 죽음 처리 오버라이드 (isDead만 설정)
    protected override void Die()
    {
        if (isDead) return;
        isDead = true;

        LogHelper.Log("지렁이가 배고파서 죽었다!");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // TODO: 게임오버 UI 표시

        Destroy(gameObject);
    }

    // ⭐ 수정: TakeDamage 오버라이드 (Hunger를 깎음)
    public override void TakeDamage(float damage)
    {
        if (isDead) return;

        // ⭐ 수정: health 대신 Hunger를 깎음
        status.TakeDamage(damage);

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        LogHelper.Log($"지렁이가 {damage} 데미지를 받음!");

        // status.Update가 알아서 죽음 처리함
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

    // ⭐ 수정: 크기 체크 추가
    public bool TryEat(Edible edible)
    {
        if (edible == null || edible.IsDead()) return false;

        float currentSize = status.GetSize(); // ⭐ 추가
        float targetRequiredSize = edible.GetRequiredSize(); // ⭐ 추가
        float targetMaxHealth = edible.GetMaxHealth();

        // ⭐ 추가: 1단계 - 크기 체크 (우선순위 높음)
        if (currentSize < targetRequiredSize)
        {
            LogHelper.Log($"{edible.GetEdibleName()}은(는) 너무 커서 먹을 수 없음! (필요 크기: {targetRequiredSize}, 현재 크기: {currentSize})");
            return false;
        }

        // ⭐ 수정: 2단계 - 데미지 체크 (한입에 먹을지 판단)
        if (biteDamage >= targetMaxHealth)
        {
            // 한입에 먹기
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

            // 죽었으면 보상
            if (edible.IsDead())
            {
                GainExp(edible.GetExpReward());
                RestoreHunger(edible.GetHungerRestore());
            }

            return false;
        }
    }

    public void TouchInedible(Inedible inedible)
    {
        if (inedible == null || inedible.IsDead()) return;

        inedible.OnTouchWorm();
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

    #region "Animation"
    private void OnSizeChanged(float _newSize)
    {
        transform.localScale = OriginScale * _newSize;

        if (wormAnimation != null)
        {
            wormAnimation.CalculateScale(_newSize);
        }
        if (wormNavi != null)
        {
            wormNavi.CalculateSpeed(_newSize);
        }
    }

    #endregion

    #region "Debug"

    public void DebugGainExp(float _Exp)
    {
        GainExp(_Exp);
    }

    public void DebugTakeDamage(float _Damage)
    {
        TakeDamage(_Damage);
    }

    public void DebugLevelUP()
    {
        status.DebugLevelUp();
    }

    // ⭐ 추가: 디버그용 크기 변경
    public void DebugSetSize(float _Size)
    {
        status.DebugSetSize(_Size);
    }

    #endregion

    private void OnDestroy()
    {
        instance = null;
    }
}