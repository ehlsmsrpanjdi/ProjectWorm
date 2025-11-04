using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SkillManager : MonoBehaviour
{
    public static SkillManager Instance;

    [Header("스킬 데이터베이스")]
    public List<SkillData> allActiveSkills;  // 9개
    public List<SkillData> allPassiveSkills; // 8개

    [Header("현재 보유 스킬")]
    public List<SkillData> ownedSkills = new List<SkillData>();

    private Dictionary<int, ActiveSkillBase> activeSkillComponents = new Dictionary<int, ActiveSkillBase>();
    private Worm worm;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        worm = Worm.Instance;
    }

    #region "레벨업 & 선택"

    // ⭐ 레벨업 시 3개 선택지 제공
    public SkillData[] GetRandomSkillOptions()
    {
        List<SkillData> availableSkills = new List<SkillData>();

        // 1. 아직 안 가진 액티브 스킬
        foreach (var skill in allActiveSkills)
        {
            if (!HasSkill(skill.skillID))
            {
                availableSkills.Add(skill);
            }
        }

        // 2. 아직 안 가진 패시브 스킬
        foreach (var skill in allPassiveSkills)
        {
            if (!HasSkill(skill.skillID))
            {
                availableSkills.Add(skill);
            }
        }

        // 3. 이미 가진 액티브 중 5강 아닌 것
        foreach (var skill in ownedSkills)
        {
            if (skill.skillType == SkillType.Active && skill.currentLevel < skill.maxLevel)
            {
                availableSkills.Add(skill);
            }
        }

        // 랜덤 3개 선택
        if (availableSkills.Count < 3)
        {
            LogHelper.LogWarrning("선택 가능한 스킬이 3개 미만!");
            return availableSkills.ToArray();
        }

        SkillData[] result = new SkillData[3];
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, availableSkills.Count);
            result[i] = availableSkills[randomIndex];
            availableSkills.RemoveAt(randomIndex);
        }

        return result;
    }

    // ⭐ 스킬 선택
    public void SelectSkill(SkillData skill)
    {
        if (HasSkill(skill.skillID))
        {
            // 이미 보유 → 레벨업
            LevelUpSkill(skill);
        }
        else
        {
            // 새로 획득
            AcquireSkill(skill);
        }
    }

    #endregion

    #region "스킬 획득 & 레벨업"

    // ⭐ 새 스킬 획득
    private void AcquireSkill(SkillData skill)
    {
        // 복사본 생성 (ScriptableObject 원본 보호)
        SkillData skillCopy = Instantiate(skill);
        skillCopy.currentLevel = (skill.skillType == SkillType.Active) ? 1 : 0;

        ownedSkills.Add(skillCopy);

        if (skill.skillType == SkillType.Active)
        {
            // 액티브 스킬 컴포넌트 추가
            AddActiveSkillComponent(skillCopy);
        }
        else
        {
            // 패시브 스킬 효과 적용
            ApplyPassiveEffect(skillCopy);
        }

        LogHelper.Log($"스킬 획득: {skill.skillName}");
    }

    // ⭐ 스킬 레벨업
    private void LevelUpSkill(SkillData skill)
    {
        SkillData ownedSkill = GetSkillByID(skill.skillID);
        if (ownedSkill == null) return;

        ownedSkill.currentLevel++;

        // 액티브 스킬 컴포넌트 업데이트
        if (activeSkillComponents.ContainsKey(skill.skillID))
        {
            activeSkillComponents[skill.skillID].OnLevelUp(ownedSkill.currentLevel);
        }

        LogHelper.Log($"스킬 레벨업: {skill.skillName} → Lv.{ownedSkill.currentLevel}");

        // 5강 달성 시 각성 체크
        if (ownedSkill.currentLevel == 5)
        {
            CheckEvolution(ownedSkill);
        }
    }

    #endregion

    #region "각성 시스템"

    // ⭐ 각성 체크
    private void CheckEvolution(SkillData skill)
    {
        if (skill.evolvedSkill == null) return;
        if (skill.requiredPassiveIDs == null || skill.requiredPassiveIDs.Length == 0) return;

        // 필요한 패시브 모두 보유했는지 체크
        bool canEvolve = true;
        foreach (int passiveID in skill.requiredPassiveIDs)
        {
            if (!HasSkill(passiveID))
            {
                canEvolve = false;
                break;
            }
        }

        if (canEvolve)
        {
            EvolveSkill(skill);
        }
    }

    // ⭐ 각성 실행
    private void EvolveSkill(SkillData originalSkill)
    {
        LogHelper.Log($"{originalSkill.skillName} 각성!");

        // 기존 스킬 제거
        RemoveSkill(originalSkill);

        // 각성 스킬 추가
        SkillData evolvedCopy = Instantiate(originalSkill.evolvedSkill);
        evolvedCopy.currentLevel = 1; // 각성 스킬은 1레벨부터 시작
        ownedSkills.Add(evolvedCopy);

        AddActiveSkillComponent(evolvedCopy);
    }

    // ⭐ 스킬 제거
    private void RemoveSkill(SkillData skill)
    {
        ownedSkills.Remove(skill);

        if (activeSkillComponents.ContainsKey(skill.skillID))
        {
            Destroy(activeSkillComponents[skill.skillID]);
            activeSkillComponents.Remove(skill.skillID);
        }
    }

    #endregion

    #region "컴포넌트 관리"

    // ⭐ 액티브 스킬 컴포넌트 추가
    private void AddActiveSkillComponent(SkillData skill)
    {
        // skillID에 따라 적절한 컴포넌트 추가
        ActiveSkillBase skillComponent = null;

        //switch (skill.skillID)
        //{
            //case 1: // 흡입
            //    skillComponent = worm.gameObject.AddComponent<VacuumSkill>();
            //    break;
            //case 2: // 충격파
            //    skillComponent = worm.gameObject.AddComponent<ShockwaveSkill>();
            //    break;
            //case 3: // 레이저
            //    skillComponent = worm.gameObject.AddComponent<LaserSkill>();
            //    break;
            //// ... 나머지 스킬들
            //default:
            //    LogHelper.LogError($"알 수 없는 스킬 ID: {skill.skillID}");
            //    return;
        //}

        if (skillComponent != null)
        {
            skillComponent.skillData = skill;
            activeSkillComponents[skill.skillID] = skillComponent;
        }
    }

    #endregion

    #region "패시브 효과"

    // ⭐ 패시브 효과 적용
    private void ApplyPassiveEffect(SkillData passive)
    {
        if (worm == null || worm.status == null) return;

        switch (passive.passiveType)
        {
            case PassiveType.AttackPower:
                worm.status.AddAttackBonus(passive.passiveValue);
                break;
            case PassiveType.Defense:
                worm.status.AddDefenseBonus(passive.passiveValue);
                break;
            case PassiveType.Currency:
                worm.status.AddCurrencyBonus(passive.passiveValue);
                break;
            case PassiveType.Digestion:
                worm.status.AddDigestionBonus(passive.passiveValue);
                break;
            case PassiveType.Experience:
                worm.status.AddExperienceBonus(passive.passiveValue);
                break;
            case PassiveType.Duration:
                worm.status.AddDurationBonus(passive.passiveValue);
                break;
            case PassiveType.Range:
                worm.status.AddRangeBonus(passive.passiveValue);
                break;
            case PassiveType.Cooldown:
                worm.status.AddCooldownBonus(passive.passiveValue);
                break;
        }

        // 모든 액티브 스킬 재계산
        foreach (var skillComponent in activeSkillComponents.Values)
        {
            skillComponent.OnPassiveChanged();
        }

        LogHelper.Log($"패시브 효과 적용: {passive.skillName}");
    }

    #endregion

    #region "유틸리티"

    private bool HasSkill(int skillID)
    {
        return ownedSkills.Exists(s => s.skillID == skillID);
    }

    private SkillData GetSkillByID(int skillID)
    {
        return ownedSkills.Find(s => s.skillID == skillID);
    }

    #endregion
}