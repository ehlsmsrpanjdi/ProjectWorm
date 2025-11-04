using System;
using UnityEngine;

public class Worm : MonoBehaviour
{
    static Worm instance;
    public static Worm Instance
    {
        get
        {
            return instance;
        }
    }


    WormStatus status = new WormStatus();

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        ComponentUpdate(deltaTime);
    }

    void ComponentUpdate(float _DeltaTime)
    {
        status.Update(_DeltaTime);
    }

    #region "StatusChange"

    void GainExp(float _Exp)
    {
        status.GainExp(_Exp);
    }

    public void ResotreHunger(float _Amount)
    {
        status.RestoreHunger(_Amount);
    }

    //public bool TryEat(Edible edible)
    //{
    //    // 크기 체크
    //    if (currentSize < edible.GetRequiredSize())
    //    {
    //        LogHelper.Log("너무 커서 먹을 수 없음!");
    //        return false;
    //    }

    //    // 먹기 성공
    //    GainExp(edible.GetExpReward());
    //    RestoreHunger(edible.GetHungerRestore());
    //    edible.OnEaten();

    //    return true;
    //}

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

    #endregion

    #region "Debug"

    public void DebugGainExp(float _Exp)
    {
        GainExp(_Exp);
    }

    #endregion

    private void OnDestroy()
    {
        instance = null;
    }
}
