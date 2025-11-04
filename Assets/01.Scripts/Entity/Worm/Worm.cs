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

    [Header("지렁이 전투 스탯")]
    public float biteDamage = 50f;

    [Header("지렁이 크기")]
    public float currentSize = 1f;


    protected override void Awake()
    {
        // ⭐ 수정: base.Awake() 호출 안 함 (health 초기화 불필요)
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

    public bool TryEat(Edible edible)
    {
        if (edible == null || edible.IsDead()) return false;

        float targetMaxHealth = edible.GetMaxHealth();

        if (biteDamage >= targetMaxHealth)
        {
            GainExp(edible.GetExpReward());
            RestoreHunger(edible.GetHungerRestore());
            edible.OnEaten();

            LogHelper.Log($"{edible.GetEdibleName()}을(를) 한입에 먹었다!");
            return true;
        }
        else
        {
            edible.TakeDamage(biteDamage);
            LogHelper.Log($"{edible.GetEdibleName()}을(를) 물었다! (데미지: {biteDamage})");

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

    #region "Debug"

    public void DebugGainExp(float _Exp)
    {
        GainExp(_Exp);
    }

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