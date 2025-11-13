using UnityEngine;
using UnityEngine.UI;
public class StatusUI : UIBase
{
    [SerializeField] Image hungerImg;
    [SerializeField] Image expImg;

    const string HungerImgString = "HungerImg";
    const string ExpImgString = "ExpImg";


    private void Reset()
    {
        hungerImg = this.TryFindChild(HungerImgString).GetComponent<Image>();
        if (hungerImg == null)
        {
            LogHelper.LogWarrning("이미지 할당 안됨");
        }

        expImg = this.TryFindChild(ExpImgString).GetComponent<Image>();
        if (expImg == null)
        {
            LogHelper.LogWarrning("이미지 할당 안됨");
        }
    }

    protected override void Start()
    {
        Init();
    }
    public void Init()
    {
        Worm.Instance.SetHungerUIBind(ChangeHungerFilled);
        Worm.Instance.SetExpUIBind(ChangeExpFilled);
        Worm.Instance.SetLevelUpUIBind(LevelUPUI);
    }

    Vector3 localScale;
    void ChangeHungerFilled(float _Ratio)
    {
        _Ratio = Mathf.Clamp(_Ratio, 0f, 1f);
        localScale = hungerImg.rectTransform.localScale;
        localScale.x = _Ratio;
        hungerImg.rectTransform.localScale = localScale;
    }

    void ChangeExpFilled(float _Ratio)
    {
        _Ratio = Mathf.Clamp(_Ratio, 0f, 1f);
        localScale = expImg.rectTransform.localScale;
        localScale.x = _Ratio;
        expImg.rectTransform.localScale = localScale;
    }

    void LevelUPUI()
    {
        SelectionUI ui = UIManager.Instance.GetUI<SelectionUI>();
        ui.OnUI();
        ui.ShowSkillOptions();
    }
}
