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

    public Action<float> OnHungryChange;
    public Action<float> OnExpchange;
    public Action<int> OnLevelUp;

    public void Reset()
    {
        Hunger = MaxHunger;
        Exp = 0f;
        Level = 1;
        CalculateMaxExp();
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

    public void GainExp(float _Exp)
    {
        Exp += _Exp;
        OnExpchange?.Invoke(Exp / MaxExp);

        while (Exp >= MaxExp)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        Exp -= MaxExp;
        Level++;
        CalculateMaxExp();

        LogHelper.Log($"레벨업! 현재 레벨: {Level}");
        OnLevelUp?.Invoke(Level);

        // ⭐ 수정: Time.timeScale 대신 이벤트로 처리
        // Time.timeScale = 0f; // 이건 삼지선다 UI에서 처리하도록
    }

    void CalculateMaxExp()
    {
        MaxExp = 100f * Mathf.Pow(expMultiplier, Level - 1);
    }

    public void RestoreHunger(float _Amount)
    {
        Hunger = Mathf.Clamp(Hunger + _Amount, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);
    }

    // ⭐ 추가: Hunger를 깎는 메서드 (데미지)
    public void TakeDamage(float _Damage)
    {
        Hunger = Mathf.Clamp(Hunger - _Damage, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);

        LogHelper.Log($"배고픔 감소! 남은 배고픔: {Hunger}/{MaxHunger}");
    }

    // Getter
    public float GetHunger() => Hunger;
    public float GetMaxHunger() => MaxHunger;
    public int GetLevel() => Level;
}