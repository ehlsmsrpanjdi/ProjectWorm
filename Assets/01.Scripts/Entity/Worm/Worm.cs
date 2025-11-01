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
