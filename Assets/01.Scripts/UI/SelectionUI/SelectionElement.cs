using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionElement : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI skillDiscription;
    [SerializeField] private SelectionRank skillRank;
    public SkillData skillData { get; private set; }

    private void Reset()
    {
        skillIcon = this.TryFindChild("SkillIcon")?.GetComponent<Image>();
        skillDiscription = this.TryFindChild("SkillDisciprtion")?.GetComponent<TextMeshProUGUI>();
    }

    private void Awake()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                LogHelper.Log("스킬 선택 버튼 클릭");
                OnClick();
            });
        }
    }

    // ⭐ 스킬 데이터 설정 (여기에 있어야 함!)
    public void SetSkillData(SkillData _skillData)
    {
        skillData = _skillData;

        if (skillData == null)
        {
            // 빈 슬롯
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        // 각성 스킬인지 체크
        bool isEvolved = SkillManager.Instance.GetPendingEvolutions().ContainsValue(skillData.skillID);

        // 아이콘 설정
        if (skillIcon != null)
        {
            if (skillData.icon != null)
            {
                skillIcon.sprite = skillData.icon;
            }

            // 각성 스킬이면 노란색
            if (isEvolved)
            {
                skillIcon.color = Color.yellow;
            }
            else
            {
                skillIcon.color = Color.white; // 기본 색상
            }
        }

        // 설명 텍스트 설정
        if (skillDiscription != null)
        {
            string displayText = skillData.GetDisplayInfo();

            if (isEvolved)
            {
                displayText = "[각성!]\n" + displayText;
            }

            skillDiscription.text = displayText;
        }

        if (skillRank != null)
        {
            skillRank.SetRank(_skillData.currentLevel + 1);
        }
    }


    private void OnClick()
    {
        if (skillData != null)
        {
            UIManager.Instance.GetUI<SelectionUI>().OnClickElement(this);
        }
    }
}