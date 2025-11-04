using UnityEngine;

public enum SkillType
{
    Active,
    Passive
}

public enum PassiveType
{
    AttackPower,      // 공격력
    Defense,          // 방어력
    Currency,         // 재화
    Digestion,        // 소화
    Experience,       // 경험치
    Duration,         // 지속시간
    Range,            // 범위
    Cooldown          // 쿨타임
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    public int skillID; // 1~9: 액티브, 10~17: 패시브
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    public SkillType skillType;

    [Header("레벨 (액티브만)")]
    public int currentLevel = 0;
    public int maxLevel = 5;

    [Header("각성 (액티브만)")]
    public int[] requiredPassiveIDs; // 필요한 패시브들
    public SkillData evolvedSkill; // 각성 버전 (별도 ScriptableObject)

    [Header("패시브 효과 (패시브만)")]
    public PassiveType passiveType;
    public float passiveValue = 0.08f; // 8%

    [Header("강화 수치 (액티브만)")]
    public float upgradeRate = 0.08f; // 8%
}