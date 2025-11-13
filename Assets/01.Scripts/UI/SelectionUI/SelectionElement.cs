using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SelectionElement : MonoBehaviour
{
    [SerializeField] Image skillIcon;
    [SerializeField] TextMeshProUGUI skillDiscription;

    public SkillData skillData;

    private void Reset()
    {
        skillIcon = this.TryFindChild("SkillIcon").GetComponent<Image>();
        skillDiscription = this.TryFindChild("SkillDisciprtion").GetComponent<TextMeshProUGUI>();
    }

    public void OnClick()
    {
        UIManager.Instance.GetUI<SelectionUI>().OnClickElement(this);
    }

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            LogHelper.Log("버튼눌림");
            OnClick();
        });
    }
}
