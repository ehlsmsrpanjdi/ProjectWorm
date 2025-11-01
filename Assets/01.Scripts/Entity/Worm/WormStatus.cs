using System;
using UnityEngine;

public class WormStatus
{
    float Hunger = 100;
    float MaxHunger = 100;
    float hungerDecreaseRate = 1f;

    float Exp = 0f;
    float MaxExp = 100;

    public Action<float> OnHungryChange;
    public Action<float> OnExpchange;

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

    public bool GainExp(float _Exp)
    {
        Exp += _Exp;
        OnExpchange?.Invoke(Exp / MaxHunger);
        if (Exp > MaxExp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
