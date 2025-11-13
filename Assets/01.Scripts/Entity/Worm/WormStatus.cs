using System;
using UnityEngine;

public class WormStatus
{
    // 배고픔 (체력 역할)
    float Hunger = 100;
    float MaxHunger = 100;
    float hungerDecreaseRate = 1f;

    // 경험치 & 레벨
    float Exp = 0f;
    float MaxExp = 100;
    int Level = 1;
    float expMultiplier = 1.5f;

    // ⭐ 추가: 크기 시스템
    float Size = 1f;                    // 현재 크기 (1.0 = 기본)
    float sizeIncreasePerLevel = 0.1f;  // 레벨당 증가량

    public Action<float> OnHungryChange;
    public Action<float> OnExpchange;
    public Action OnLevelUp;
    public Action<float> OnSizeChange; // ⭐ 추가: 크기 변경 이벤트


    private float attackMultiplier = 1f;
    private float defenseMultiplier = 1f;
    private float currencyMultiplier = 1f;
    private float digestionMultiplier = 1f;
    private float experienceMultiplier = 1f;
    private float durationMultiplier = 1f;
    private float rangeMultiplier = 1f;
    private float cooldownMultiplier = 1f;

    public void Reset()
    {
        Hunger = MaxHunger;
        Exp = 0f;
        Level = 1;
        Size = 1f; // ⭐ 추가
        CalculateMaxExp();
    }

    public void SetScaleRatio(float _Ratio)
    {
        Size *= _Ratio;
        sizeIncreasePerLevel *= _Ratio;
    }

    public bool Update(float _DeltaTime)
    {
        Hunger = Mathf.Clamp(Hunger - hungerDecreaseRate * _DeltaTime, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);

        if (Hunger <= 0)
        {
            return false;
        }
        return true;
    }

    #region "Gain"

    // ⭐ 수정: 경험치 획득 시 보너스 적용
    public void GainExp(float _Exp)
    {
        _Exp *= experienceMultiplier; // ⭐ 추가
        Exp += _Exp;
        OnExpchange?.Invoke(Exp / MaxExp);

        while (Exp >= MaxExp)
        {
            LevelUp();
        }
    }

    // ⭐ 수정: 배고픔 회복 시 보너스 적용
    public void RestoreHunger(float _Amount)
    {
        _Amount *= digestionMultiplier; // ⭐ 추가
        Hunger = Mathf.Clamp(Hunger + _Amount, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);
    }


    #endregion

    #region "LevelUP"

    void LevelUp()
    {
        Exp -= MaxExp;
        Level++;
        CalculateMaxExp();

        // ⭐ 추가: 레벨업 시 크기 증가
        IncreaseSize(sizeIncreasePerLevel);

        LogHelper.Log($"레벨업! 현재 레벨: {Level}, 크기: {Size}");
        OnLevelUp?.Invoke();
    }

    public void IncreaseSize(float _Amount)
    {
        Size += _Amount;
        OnSizeChange?.Invoke(Size);
        LogHelper.Log($"크기 증가! 현재 크기: {Size}");
    }

    void CalculateMaxExp()
    {
        MaxExp = 100f * Mathf.Pow(expMultiplier, Level - 1);
    }

    #endregion

    // ⭐ 수정: 데미지 받을 때 방어력 적용
    public void TakeDamage(float _Damage)
    {
        _Damage *= defenseMultiplier; // ⭐ 추가
        Hunger = Mathf.Clamp(Hunger - _Damage, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);

        LogHelper.Log($"배고픔 감소! 남은 배고픔: {Hunger}/{MaxHunger}");
    }

    #region "Status

    // ⭐ 추가: 패시브 보너스 적용 메서드들
    public void AddAttackBonus(float value)
    {
        attackMultiplier *= (1f + value);
        LogHelper.Log($"공격력 보너스: {attackMultiplier * 100f}%");
    }

    public void AddDefenseBonus(float value)
    {
        defenseMultiplier *= (1f - value); // 받는 데미지 감소
        LogHelper.Log($"방어력 보너스: {defenseMultiplier * 100f}%");
    }

    public void AddCurrencyBonus(float value)
    {
        currencyMultiplier *= (1f + value);
    }

    public void AddDigestionBonus(float value)
    {
        digestionMultiplier *= (1f + value);
    }

    public void AddExperienceBonus(float value)
    {
        experienceMultiplier *= (1f + value);
    }

    public void AddDurationBonus(float value)
    {
        durationMultiplier *= (1f + value);
    }

    public void AddRangeBonus(float value)
    {
        rangeMultiplier *= (1f + value);
    }

    public void AddCooldownBonus(float value)
    {
        cooldownMultiplier *= (1f - value); // 쿨타임 감소
    }

    #endregion

    #region "Debug"

    public void DebugSetSize(float _Size)
    {
        Size = _Size;
        OnSizeChange?.Invoke(Size);
        LogHelper.Log($"크기 설정! 현재 크기: {Size}");
    }

    public void DebugLevelUp()
    {
        LevelUp();
    }

    #endregion



    // Getter
    public float GetHunger() => Hunger;
    public float GetMaxHunger() => MaxHunger;
    public int GetLevel() => Level;
    public float GetSize() => Size; // ⭐ 추가

    public float GetAttackMultiplier() => attackMultiplier;
    public float GetDefenseMultiplier() => defenseMultiplier;
    public float GetCurrencyMultiplier() => currencyMultiplier;
    public float GetDigestionMultiplier() => digestionMultiplier;
    public float GetExperienceMultiplier() => experienceMultiplier;
    public float GetDurationMultiplier() => durationMultiplier;
    public float GetRangeMultiplier() => rangeMultiplier;
    public float GetCooldownMultiplier() => cooldownMultiplier;

}