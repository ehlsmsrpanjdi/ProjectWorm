using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    private static SkillManager instance;
    public static SkillManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SkillManager();
                instance.Init();
            }
            return instance;
        }
    }

    public List<SkillData> ownedSkills = new List<SkillData>();

    // ⭐ 추가: 각성 대기 중인 스킬들 (원본 스킬 ID → 각성 스킬 ID)
    private Dictionary<int, int> pendingEvolutions = new Dictionary<int, int>();

    private Dictionary<int, ActiveSkillBase> activeSkillComponents = new Dictionary<int, ActiveSkillBase>();
    private Worm worm;

    private void Init()
    {
        worm = Worm.Instance;
        var dataManager = SkillDataManager.Instance;
        LogHelper.Log("SkillManager 초기화 완료");
    }

    #region "레벨업 & 선택"

    public SkillData[] GetRandomSkillOptions()
    {
        List<SkillData> availableSkills = new List<SkillData>();

        // ⭐ 1. 각성 대기 중인 스킬 추가 (최우선)
        foreach (var kvp in pendingEvolutions)
        {
            int evolvedSkillID = kvp.Value;
            SkillData evolvedSkill = SkillDataManager.Instance.GetSkillByID(evolvedSkillID);
            if (evolvedSkill != null)
            {
                availableSkills.Add(evolvedSkill);
                LogHelper.Log($"각성 스킬 선택지 추가: {evolvedSkill.skillName}");
            }
        }

        List<SkillData> allActives = SkillDataManager.Instance.GetAllActiveSkills();
        List<SkillData> allPassives = SkillDataManager.Instance.GetAllPassiveSkills();

        // 2. 아직 안 가진 액티브 스킬
        foreach (var skill in allActives)
        {
            if (!HasSkill(skill.skillID))
            {
                availableSkills.Add(skill);
            }
        }

        // 3. 아직 안 가진 패시브 스킬
        foreach (var skill in allPassives)
        {
            if (!HasSkill(skill.skillID))
            {
                availableSkills.Add(skill);
            }
        }

        // 4. 이미 가진 액티브 중 5강 아닌 것
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
        List<SkillData> tempList = new List<SkillData>(availableSkills);

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            result[i] = tempList[randomIndex];
            tempList.RemoveAt(randomIndex);
        }

        return result;
    }

    public void SelectSkill(SkillData skill)
    {
        // ⭐ 각성 스킬인지 체크
        if (IsEvolvedSkill(skill))
        {
            ExecutePendingEvolution(skill);
        }
        else if (HasSkill(skill.skillID))
        {
            LevelUpSkill(skill);
        }
        else
        {
            AcquireSkill(skill);
        }

        LogHelper.Log($"스킬 선택 완료: {skill.skillName}");
    }

    #endregion

    #region "스킬 획득 & 레벨업"

    private void AcquireSkill(SkillData skill)
    {
        SkillData skillCopy = skill.Clone();
        skillCopy.currentLevel = (skill.skillType == SkillType.Active) ? 1 : 0;

        ownedSkills.Add(skillCopy);

        if (skill.skillType == SkillType.Active)
        {
            AddActiveSkillComponent(skillCopy);
        }
        else
        {
            ApplyPassiveEffect(skillCopy);
            // ⭐ 패시브 획득 시 각성 가능 체크
            CheckPendingEvolutionsAfterPassive(skillCopy);
        }

        LogHelper.Log($"스킬 획득: {skill.skillName}");
    }

    private void LevelUpSkill(SkillData skill)
    {
        SkillData ownedSkill = GetSkillByID(skill.skillID);
        if (ownedSkill == null) return;

        ownedSkill.currentLevel++;

        if (activeSkillComponents.ContainsKey(skill.skillID))
        {
            activeSkillComponents[skill.skillID].OnLevelUp(ownedSkill.currentLevel);
        }

        LogHelper.Log($"스킬 레벨업: {skill.skillName} → Lv.{ownedSkill.currentLevel}");

        // ⭐ 5강 달성 시 각성 가능 체크
        if (ownedSkill.currentLevel == 5)
        {
            CheckPendingEvolutionsAfterLevelUp(ownedSkill);
        }
    }

    #endregion

    #region "각성 시스템"

    // ⭐ 액티브 5강 달성 시 각성 가능 체크
    private void CheckPendingEvolutionsAfterLevelUp(SkillData skill)
    {
        if (skill.skillType != SkillType.Active) return;

        List<int> ownedSkillIDs = ownedSkills.ConvertAll(s => s.skillID);

        foreach (var combo in skill.combinations)
        {
            if (combo.CanCombine(ownedSkillIDs))
            {
                // 조합 가능!
                if (!pendingEvolutions.ContainsKey(skill.skillID))
                {
                    pendingEvolutions.Add(skill.skillID, combo.evolvedSkillID);
                    LogHelper.Log($"{skill.skillName} 각성 대기 상태! (필요 조건 충족)");
                }
            }
        }
    }

    // ⭐ 패시브 획득 시 각성 가능 체크
    private void CheckPendingEvolutionsAfterPassive(SkillData newPassive)
    {
        if (newPassive.skillType != SkillType.Passive) return;

        // 이 패시브를 필요로 하는 액티브들 확인
        List<int> relatedActives = SkillDataManager.Instance.GetActiveSkillsUsingPassive(newPassive.skillID);

        foreach (int activeID in relatedActives)
        {
            SkillData ownedActive = GetSkillByID(activeID);

            // 보유하고 있고 5강인가?
            if (ownedActive != null && ownedActive.currentLevel == 5)
            {
                List<int> ownedSkillIDs = ownedSkills.ConvertAll(s => s.skillID);

                foreach (var combo in ownedActive.combinations)
                {
                    if (combo.CanCombine(ownedSkillIDs))
                    {
                        // 조합 가능!
                        if (!pendingEvolutions.ContainsKey(ownedActive.skillID))
                        {
                            pendingEvolutions.Add(ownedActive.skillID, combo.evolvedSkillID);
                            LogHelper.Log($"{ownedActive.skillName} 각성 대기 상태! ({newPassive.skillName} 획득으로 조건 충족)");
                        }
                    }
                }
            }
        }
    }

    // ⭐ 각성 스킬 선택 시 실행
    private void ExecutePendingEvolution(SkillData evolvedSkill)
    {
        // 어떤 원본 스킬의 각성인지 찾기
        int originalSkillID = -1;
        foreach (var kvp in pendingEvolutions)
        {
            if (kvp.Value == evolvedSkill.skillID)
            {
                originalSkillID = kvp.Key;
                break;
            }
        }

        if (originalSkillID == -1)
        {
            LogHelper.LogError("각성 대기 목록에서 찾을 수 없음!");
            return;
        }

        SkillData originalSkill = GetSkillByID(originalSkillID);
        if (originalSkill == null) return;

        LogHelper.Log($"{originalSkill.skillName} → {evolvedSkill.skillName} 각성!");

        // 기존 스킬 제거
        RemoveSkill(originalSkill);

        // 각성 스킬 추가
        SkillData evolvedCopy = evolvedSkill.Clone();
        evolvedCopy.currentLevel = 1;
        ownedSkills.Add(evolvedCopy);
        AddActiveSkillComponent(evolvedCopy);

        // 각성 대기 목록에서 제거
        pendingEvolutions.Remove(originalSkillID);
    }

    private void RemoveSkill(SkillData skill)
    {
        ownedSkills.Remove(skill);

        if (activeSkillComponents.ContainsKey(skill.skillID))
        {
            Object.Destroy(activeSkillComponents[skill.skillID]);
            activeSkillComponents.Remove(skill.skillID);
        }
    }

    // ⭐ 각성 스킬인지 체크
    private bool IsEvolvedSkill(SkillData skill)
    {
        return pendingEvolutions.ContainsValue(skill.skillID);
    }

    #endregion

    #region "컴포넌트 관리"

    private void AddActiveSkillComponent(SkillData skill)
    {
        if (worm == null) return;

        ActiveSkillBase skillComponent = null;

        switch (skill.skillID)
        {
            case 1: // 흡입
                LogHelper.Log("흡입 스킬 컴포넌트 추가 (미구현)");
                break;
            case 2: // 충격파
                LogHelper.Log("충격파 스킬 컴포넌트 추가 (미구현)");
                break;
            default:
                if (skill.skillID >= 100) // 각성 스킬
                {
                    LogHelper.Log($"각성 스킬 ID {skill.skillID} 추가 (미구현)");
                }
                else
                {
                    LogHelper.LogWarrning($"알 수 없는 스킬 ID: {skill.skillID}");
                }
                return;
        }

        if (skillComponent != null)
        {
            skillComponent.skillData = skill;
            activeSkillComponents[skill.skillID] = skillComponent;
        }
    }

    #endregion

    #region "패시브 효과"

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

        foreach (var skillComponent in activeSkillComponents.Values)
        {
            skillComponent.OnPassiveChanged();
        }

        LogHelper.Log($"패시브 효과 적용: {passive.skillName}");
    }

    #endregion

    #region "유틸리티"

    public bool HasSkill(int skillID)
    {
        return ownedSkills.Exists(s => s.skillID == skillID);
    }

    public SkillData GetSkillByID(int skillID)
    {
        return ownedSkills.Find(s => s.skillID == skillID);
    }

    public List<SkillData> GetOwnedSkills()
    {
        return new List<SkillData>(ownedSkills);
    }

    // ⭐ 추가: 각성 대기 중인 스킬 확인
    public Dictionary<int, int> GetPendingEvolutions()
    {
        return new Dictionary<int, int>(pendingEvolutions);
    }

    #endregion
}