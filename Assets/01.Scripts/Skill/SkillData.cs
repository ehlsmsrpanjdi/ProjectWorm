using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Active,
    Passive
}

public enum PassiveType
{
    AttackPower,
    Defense,
    Currency,
    Digestion,
    Experience,
    Duration,
    Range,
    Cooldown
}

// 조합 정보 클래스
[Serializable]
public class CombinationInfo
{
    public int[] requiredSkillIDs; // 필요한 스킬들
    public int evolvedSkillID;     // 각성 후 스킬 ID

    public CombinationInfo(int[] required, int evolved)
    {
        requiredSkillIDs = required;
        evolvedSkillID = evolved;
    }

    // 조합 가능 체크
    public bool CanCombine(List<int> ownedSkillIDs)
    {
        foreach (int requiredID in requiredSkillIDs)
        {
            if (!ownedSkillIDs.Contains(requiredID))
                return false;
        }
        return true;
    }
}

[Serializable]
public class SkillData
{
    // 기본 정보
    public int skillID;
    public string skillName;
    public string description;
    public Sprite icon;
    public SkillType skillType;

    // 레벨 (액티브만)
    public int currentLevel = 0;
    public int maxLevel = 5;

    // 조합 정보 (여러 개 가능)
    public List<CombinationInfo> combinations = new List<CombinationInfo>();

    // 역참조 (이 스킬을 필요로 하는 액티브들)
    public List<int> usedInCombinationsBy = new List<int>();

    // 패시브 효과 (패시브만)
    public PassiveType passiveType;
    public float passiveValue = 0.08f;

    // 강화 수치 (액티브만)
    public float upgradeRate = 0.08f;

    // 생성자
    public SkillData(int id, string name, string desc, SkillType type)
    {
        skillID = id;
        skillName = name;
        description = desc;
        skillType = type;
    }

    // 조합 추가 헬퍼
    public void AddCombination(int[] requiredIDs, int evolvedID)
    {
        combinations.Add(new CombinationInfo(requiredIDs, evolvedID));
    }

    // UI 표시용
    public string GetDisplayInfo()
    {
        if (skillType == SkillType.Active)
        {
            return $"{skillName} Lv.{currentLevel}/{maxLevel}\n{description}";
        }
        else
        {
            return $"{skillName}\n{description}\n+{passiveValue * 100}%";
        }
    }

    // 복사본 생성
    public SkillData Clone()
    {
        SkillData copy = new SkillData(skillID, skillName, description, skillType);
        copy.icon = this.icon;
        copy.currentLevel = this.currentLevel;
        copy.maxLevel = this.maxLevel;
        copy.combinations = new List<CombinationInfo>(this.combinations);
        copy.usedInCombinationsBy = new List<int>(this.usedInCombinationsBy);
        copy.passiveType = this.passiveType;
        copy.passiveValue = this.passiveValue;
        copy.upgradeRate = this.upgradeRate;
        return copy;
    }
}