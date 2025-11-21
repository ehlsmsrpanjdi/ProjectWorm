using TMPro;
using UnityEditor;
using UnityEngine;

public class DebugWindow : EditorWindow
{
    TMP_FontAsset targetFont;



    private float debugFloatValue = 0f;



    [MenuItem("Window/DebugWindow")]
    public static void ShowWindow()
    {
        // 창 생성
        GetWindow<DebugWindow>("My Editor");
    }

    // 창에 그려질 UI
    void OnGUI()
    {
        GUILayout.Label("TextMeshPro Font Replacer", EditorStyles.boldLabel);
        targetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Target Font Asset", targetFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace Fonts in Scene"))
        {
            if (targetFont == null)
            {
                EditorUtility.DisplayDialog("Error", "Target Font Asset을 지정하세요!", "확인");
                return;
            }

            ReplaceAllTMPFonts(targetFont);
        }


        #region "Skill"

        if (GUILayout.Button("Exp Gain"))
        {
            Worm.Instance.DebugGainExp(10f);
        }

        if (GUILayout.Button("LevelUP"))
        {
            Worm.Instance.DebugLevelUP();
        }

        if (GUILayout.Button("Laser"))
        {
            SkillManager.Instance.SelectSkill(SkillDataManager.Instance.GetSkillByID(3));
        }

        if (GUILayout.Button("AirCannon"))
        {
            SkillManager.Instance.SelectSkill(SkillDataManager.Instance.GetSkillByID(9));
        }

        if (GUILayout.Button("ShokeWave"))
        {
            SkillManager.Instance.SelectSkill(SkillDataManager.Instance.GetSkillByID(2));
        }

        if (GUILayout.Button("Virus"))
        {
            SkillManager.Instance.SelectSkill(SkillDataManager.Instance.GetSkillByID(6));
        }


        if (GUILayout.Button("Vaccum"))
        {
            SkillManager.Instance.SelectSkill(SkillDataManager.Instance.GetSkillByID(1));
        }

        if (GUILayout.Button("SandStorm"))
        {
            SkillManager.Instance.SelectSkill(SkillDataManager.Instance.GetSkillByID(10));
        }

        #endregion

        GUILayout.Space(10);
        GUILayout.Label("DebugFloatValue", EditorStyles.boldLabel);

        debugFloatValue = EditorGUILayout.FloatField("입력값 : ", debugFloatValue);

        if (GUILayout.Button("worm dead"))
        {
            Worm.Instance.TakeDamage(1000);
        }


        if (GUILayout.Button("Calculate Scale"))
        {
            Worm.Instance.DebugSizeChange(debugFloatValue);
        }

        if (GUILayout.Button("Calculate WormAnim"))
        {
            Worm.Instance.DebugCalculateAnim(debugFloatValue);
        }

    }

    void ReplaceAllTMPFonts(TMP_FontAsset newFont)
    {
        //int count = 0;
        //var texts = FindObjectsOfType<TextMeshProUGUI>(true); // 비활성화 오브젝트 포함
        //foreach (var tmp in texts)
        //{
        //    Undo.RecordObject(tmp, "Change TMP Font");

        //    tmp.font = newFont;
        //    tmp.fontSharedMaterial = newFont.material; // 머티리얼도 reset
        //    tmp.UpdateFontAsset();

        //    EditorUtility.SetDirty(tmp);
        //    count++;
        //}

        //Debug.Log($"✅ Scene 내 모든 TextMeshProUGUI {count}개를 '{newFont.name}' 폰트로 교체 완료!");
    }

}
