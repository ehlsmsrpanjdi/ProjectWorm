using UnityEngine;

public class ThirdMonsterMaterialSetting : MonoBehaviour
{
    SkinnedMeshRenderer renderTexture;

    private void Start()
    {
        renderTexture = GetComponent<SkinnedMeshRenderer>();
        renderTexture.updateWhenOffscreen = true;
    }
}
