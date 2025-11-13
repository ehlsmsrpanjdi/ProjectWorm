using System.Collections.Generic;

public class SkillDataManager
{
    private static SkillDataManager instance;
    public static SkillDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SkillDataManager();
                instance.Init();
            }
            return instance;
        }
    }

    private Dictionary<int, SkillData> skillDatabase = new Dictionary<int, SkillData>();

    private void Init()
    {
        CreateAllSkills();
        BuildCombinationReferences();
        LogHelper.Log($"스킬 데이터 로드 완료: {skillDatabase.Count}개");
    }

    private void CreateAllSkills()
    {
        CreateActiveSkills();
        CreatePassiveSkills();
        CreateEvolvedSkills();
    }

    #region "액티브 스킬 생성"

    private void CreateActiveSkills()
    {
        // 1. 전방 흡입
        SkillData vacuum = new SkillData(1, "전방 흡입", "입을 기준으로 회오리가 생겨 주변의 적을 빨아들입니다", SkillType.Active);
        vacuum.AddCombination(new int[] { 106 }, 201); // 효율적인 공격 (범위)
        skillDatabase.Add(1, vacuum);

        // 2. 충격파
        SkillData shockwave = new SkillData(2, "충격파", "지렁이 중심으로 충격파가 발생합니다", SkillType.Active);
        shockwave.AddCombination(new int[] { 107 }, 202); // 더 빠른 공격 (쿨타임)
        skillDatabase.Add(2, shockwave);

        // 3. 레이저
        SkillData laser = new SkillData(3, "레이저 발사", "입에서 레이저를 발사합니다", SkillType.Active);
        laser.AddCombination(new int[] { 100 }, 203); // 공격력 증가
        skillDatabase.Add(3, laser);

        // 4. 소환 A
        SkillData summonA = new SkillData(4, "작은 지렁이 A 소환", "플레이어를 따라다니며 주변의 적을 공격합니다", SkillType.Active);
        summonA.AddCombination(new int[] { 5 }, 204); // 소환 B와 조합 (액티브+액티브)
        skillDatabase.Add(4, summonA);

        // 5. 소환 B
        SkillData summonB = new SkillData(5, "작은 지렁이 B 소환", "적을 따라다니며 주변의 적을 공격합니다", SkillType.Active);
        summonB.AddCombination(new int[] { 4 }, 204); // 소환 A와 조합 (액티브+액티브)
        skillDatabase.Add(5, summonB);

        // 6. 염산 장판
        SkillData acidTrail = new SkillData(6, "염산 장판", "지나간 자리에 염산이 남습니다", SkillType.Active);
        acidTrail.AddCombination(new int[] { 105 }, 206); // 효과적인 공격 (지속시간)
        skillDatabase.Add(6, acidTrail);

        // 7. 가시 발사
        SkillData spineShot = new SkillData(7, "가시 발사", "지렁이 몸에서 가시를 발사합니다", SkillType.Active);
        spineShot.AddCombination(new int[] { 100 }, 207); // 공격력 증가
        skillDatabase.Add(7, spineShot);

        // 8. 가시 방어
        SkillData thornArmor = new SkillData(8, "가시 방어", "몸 주변에 가시가 자랍니다", SkillType.Active);
        thornArmor.AddCombination(new int[] { 101 }, 208); // 방어력 증가
        skillDatabase.Add(8, thornArmor);

        // 9. 진공파
        SkillData vacuumBall = new SkillData(9, "진공파 발사", "입에서 진공파 투사체를 발사합니다", SkillType.Active);
        vacuumBall.AddCombination(new int[] { 106 }, 209); // 효율적인 공격 (범위)
        skillDatabase.Add(9, vacuumBall);

        // 10. 모래 소용돌이
        SkillData sandstorm = new SkillData(10, "모래 소용돌이", "주변에 랜덤한 위치에 모래 소용돌이가 발생합니다", SkillType.Active);
        sandstorm.AddCombination(new int[] { 105 }, 210); // 효과적인 공격 (지속시간)
        skillDatabase.Add(10, sandstorm);
    }

    #endregion

    #region "패시브 스킬 생성"

    private void CreatePassiveSkills()
    {
        // 100. 공격력 증가
        SkillData attackPower = new SkillData(100, "공격력 증가", "모든 데미지가 증가합니다", SkillType.Passive);
        attackPower.passiveType = PassiveType.AttackPower;
        attackPower.passiveValue = 0.08f;
        skillDatabase.Add(100, attackPower);

        // 101. 방어력 증가
        SkillData defense = new SkillData(101, "방어력 증가", "받는 데미지가 감소합니다", SkillType.Passive);
        defense.passiveType = PassiveType.Defense;
        defense.passiveValue = 0.08f;
        skillDatabase.Add(101, defense);

        // 102. 재화 증가
        SkillData currency = new SkillData(102, "전투 중 얻는 재화 증가", "전투에서 얻는 재화가 증가합니다", SkillType.Passive);
        currency.passiveType = PassiveType.Currency;
        currency.passiveValue = 0.08f;
        skillDatabase.Add(102, currency);

        // 103. 효율적 소화
        SkillData digestion = new SkillData(103, "효율적 소화", "먹었을 때 배고픔 회복량이 증가합니다", SkillType.Passive);
        digestion.passiveType = PassiveType.Digestion;
        digestion.passiveValue = 0.08f;
        skillDatabase.Add(103, digestion);

        // 104. 효율적 경험
        SkillData experience = new SkillData(104, "효율적 경험", "경험치 획득량이 증가합니다", SkillType.Passive);
        experience.passiveType = PassiveType.Experience;
        experience.passiveValue = 0.08f;
        skillDatabase.Add(104, experience);

        // 105. 효과적인 공격 (지속시간)
        SkillData duration = new SkillData(105, "효과적인 공격", "스킬 지속시간이 증가합니다", SkillType.Passive);
        duration.passiveType = PassiveType.Duration;
        duration.passiveValue = 0.08f;
        skillDatabase.Add(105, duration);

        // 106. 효율적인 공격 (범위)
        SkillData range = new SkillData(106, "효율적인 공격", "스킬 범위가 증가합니다", SkillType.Passive);
        range.passiveType = PassiveType.Range;
        range.passiveValue = 0.08f;
        skillDatabase.Add(106, range);

        // 107. 더 빠른 공격 (쿨타임)
        SkillData cooldown = new SkillData(107, "더 빠른 공격", "스킬 쿨타임이 감소합니다", SkillType.Passive);
        cooldown.passiveType = PassiveType.Cooldown;
        cooldown.passiveValue = 0.08f;
        skillDatabase.Add(107, cooldown);
    }

    #endregion

    #region "각성 스킬 생성"

    private void CreateEvolvedSkills()
    {
        // 201. 전방 흡입 → 블랙홀
        SkillData blackhole = new SkillData(201, "블랙홀", "영구적으로 회오리를 유지하며 모든 것을 빨아들입니다", SkillType.Active);
        skillDatabase.Add(201, blackhole);

        // 202. 충격파 → 지진
        SkillData earthquake = new SkillData(202, "지진", "적을 즉사시키는 강력한 충격파를 발생시킵니다", SkillType.Active);
        skillDatabase.Add(202, earthquake);

        // 203. 레이저 → 파괴광선
        SkillData destroyBeam = new SkillData(203, "파괴광선", "범위와 데미지가 극대화된 레이저를 발사합니다", SkillType.Active);
        skillDatabase.Add(203, destroyBeam);

        // 204. 소환 A+B → 군단
        SkillData legion = new SkillData(204, "군단", "소환수들이 플레이어의 스킬을 사용합니다", SkillType.Active);
        skillDatabase.Add(204, legion);

        // 206. 염산 → 박테리아
        SkillData bacteria = new SkillData(206, "박테리아", "적을 추적하는 박테리아가 몸 주변에서 계속 생성됩니다", SkillType.Active);
        skillDatabase.Add(206, bacteria);

        // 207. 가시 발사 → 가시 폭풍
        SkillData spineStorm = new SkillData(207, "가시 폭풍", "데미지는 감소하지만 끊임없이 가시를 발사합니다", SkillType.Active);
        skillDatabase.Add(207, spineStorm);

        // 208. 가시 방어 → 철갑 지렁이
        SkillData ironArmor = new SkillData(208, "철갑 지렁이", "가시 크기가 증가하고 적에게 중독과 강한 슬로우를 줍니다", SkillType.Active);
        skillDatabase.Add(208, ironArmor);

        // 209. 진공파 → 파멸의 구체
        SkillData doomSphere = new SkillData(209, "파멸의 구체", "하나의 거대한 구체가 맞으면 폭발합니다", SkillType.Active);
        skillDatabase.Add(209, doomSphere);

        // 210. 모래 소용돌이 → 사막 폭풍
        SkillData desertStorm = new SkillData(210, "사막 폭풍", "모래 소용돌이가 플레이어를 따라다닙니다", SkillType.Active);
        skillDatabase.Add(210, desertStorm);

        LogHelper.Log("각성 스킬 생성 완료");
    }

    #endregion

    #region "역참조 빌드"

    private void BuildCombinationReferences()
    {
        foreach (var skill in skillDatabase.Values)
        {
            if (skill.skillType == SkillType.Active)
            {
                foreach (var combo in skill.combinations)
                {
                    foreach (int requiredID in combo.requiredSkillIDs)
                    {
                        if (skillDatabase.ContainsKey(requiredID))
                        {
                            skillDatabase[requiredID].usedInCombinationsBy.Add(skill.skillID);
                        }
                    }
                }
            }
        }

        LogHelper.Log("조합 역참조 빌드 완료");
    }

    #endregion

    #region "데이터 접근"

    public SkillData GetSkillByID(int skillID)
    {
        if (skillDatabase.ContainsKey(skillID))
        {
            return skillDatabase[skillID];
        }

        LogHelper.LogError($"스킬 ID {skillID}를 찾을 수 없음!");
        return null;
    }

    public List<SkillData> GetAllActiveSkills()
    {
        List<SkillData> actives = new List<SkillData>();
        foreach (var skill in skillDatabase.Values)
        {
            if (skill.skillType == SkillType.Active && skill.skillID < 100)
            {
                actives.Add(skill);
            }
        }
        return actives;
    }

    public List<SkillData> GetAllPassiveSkills()
    {
        List<SkillData> passives = new List<SkillData>();
        foreach (var skill in skillDatabase.Values)
        {
            if (skill.skillType == SkillType.Passive)
            {
                passives.Add(skill);
            }
        }
        return passives;
    }

    // 특정 패시브가 어떤 액티브와 조합되는지
    public List<int> GetActiveSkillsUsingPassive(int passiveID)
    {
        if (skillDatabase.ContainsKey(passiveID))
        {
            return skillDatabase[passiveID].usedInCombinationsBy;
        }
        return new List<int>();
    }

    #endregion
}
