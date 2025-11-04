using System;
using UnityEngine;

public class WormStatus
{
    float Hunger = 100;
    float MaxHunger = 100;
    float hungerDecreaseRate = 1f;

    // 경험치 & 레벨
    float Exp = 0f;
    float MaxExp = 100;
    int Level = 1;                    // ⭐ 추가: 레벨
    float expMultiplier = 1.5f;       // ⭐ 추가: 레벨업마다 필요 경험치 배율

    public Action<float> OnHungryChange;
    public Action<float> OnExpchange;
    public Action<int> OnLevelUp;     // ⭐ 추가: 레벨업 이벤트


    public void Reset()
    {
        Hunger = 100;
    }

    public bool Update(float _DeltaTime)
    {
        Hunger = Mathf.Clamp(Hunger - hungerDecreaseRate * _DeltaTime, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);
        if (Hunger == 0)
        {
            return false;
        }
        return true;
    }


    public void GainExp(float _Exp)
    {
        Exp += _Exp;
        OnExpchange?.Invoke(Exp / MaxExp);

        // 레벨업 체크
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

        // 게임 일시정지 & UI 띄우기
        Time.timeScale = 0f;
        // TODO: 삼지선다 UI 표시
    }


    void CalculateMaxExp()
    {
        MaxExp = 100f * Mathf.Pow(expMultiplier, Level - 1);
    }

    // ⭐ 추가: 배고픔 회복 (먹이 먹었을 때)
    public void RestoreHunger(float _Amount)
    {
        Hunger = Mathf.Clamp(Hunger + _Amount, 0, MaxHunger);
        OnHungryChange?.Invoke(Hunger / MaxHunger);
    }

    // ⭐ 추가: Getter 메서드들
    public float GetHunger() => Hunger;
    public float GetMaxHunger() => MaxHunger;
    public int GetLevel() => Level;
}
