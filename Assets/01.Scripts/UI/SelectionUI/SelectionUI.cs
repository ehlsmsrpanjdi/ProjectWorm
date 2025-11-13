using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionUI : UIBase
{
    [Header("선택지 Element들")]
    [SerializeField] private List<SelectionElement> selectionElements;

    private void Reset()
    {
        selectionElements = GetComponentsInChildren<SelectionElement>().ToList();
    }

    protected override void Awake()
    {
        base.Awake();

        // Element 찾기
        if (selectionElements == null || selectionElements.Count == 0)
        {
            selectionElements = GetComponentsInChildren<SelectionElement>(true).ToList();
        }

        if (selectionElements.Count != 3)
        {
            LogHelper.LogWarrning($"SelectionElement 개수가 3개가 아님: {selectionElements.Count}개");
        }

        OffUI();
    }

    // 레벨업 시 호출될 메서드
    public void ShowSkillOptions()
    {
        // 게임 일시정지
        Time.timeScale = 0f;

        // SkillManager에서 3개 선택지 가져오기
        SkillData[] options = SkillManager.Instance.GetRandomSkillOptions();

        // 각 Element에 데이터 설정
        for (int i = 0; i < selectionElements.Count; i++)
        {
            if (i < options.Length)
            {
                selectionElements[i].SetSkillData(options[i]);
            }
            else
            {
                selectionElements[i].SetSkillData(null); // 빈 슬롯
            }
        }

        // UI 표시
        OnUI();

        LogHelper.Log("스킬 선택 UI 표시");
    }

    // Element 클릭 시
    public void OnClickElement(SelectionElement _Element)
    {
        if (_Element.skillData == null) return;

        // 스킬 선택
        SkillManager.Instance.SelectSkill(_Element.skillData);

        // UI 닫기
        CloseSelection();
    }

    // 선택 완료 후
    private void CloseSelection()
    {
        // 게임 재개
        Time.timeScale = 1f;

        // UI 닫기
        OffUI();

        LogHelper.Log("스킬 선택 완료, 게임 재개");
    }
}