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
    public Action<int> OnLevelUp;
    public Action<float> OnSizeChange; // ⭐ 추가: 크기 변경 이벤트

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

    public void GainExp(float _Exp)
    {
        Exp += _Exp;
        OnExpchange?.Invoke(Exp / MaxExp);

        while (Exp >= MaxExp)
        {
            LevelUp();
        }
    }

    public void RestoreHunger(float _Amount)
    {
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
        OnLevelUp?.Invoke(Level);
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

    public void TakeDamage(float _Damage)
    {
        Hunger = Mathf.Clamp(Hunger - _Damage, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);

        LogHelper.Log($"배고픔 감소! 남은 배고픔: {Hunger}/{MaxHunger}");
    }



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
}