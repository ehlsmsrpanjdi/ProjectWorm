using System.Collections;
using UnityEngine;

public class HitAction3D : MonoBehaviour
{
    [Header("맞았을 경우 변경할 마테리얼")]
    [SerializeField] private Material hitMaterial;
    private Material[] hitMaterials;

    [SerializeField] private SkinnedMeshRenderer render;
    private Material[] monsterMaterial;

    private Coroutine coroutine;

#if UNITY_EDITOR
    private void Reset()
    {
        render = this.TryGetComponent<SkinnedMeshRenderer>();
        if (!render) render = this.TryGetChildComponent<SkinnedMeshRenderer>();
    }
#endif

    private void Start()
    {
        monsterMaterial = render.materials;

        hitMaterials = new Material[render.materials.Length];
        var setHitMatherial = new Material[hitMaterials.Length];

        for (int i = 0; i < setHitMatherial.Length; i++)
        {
            setHitMatherial[i] = hitMaterial;
        }

        hitMaterials = setHitMatherial;
    }

    public void Hit()
    {
        render.materials = hitMaterials;

        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(TimeOver());
    }

    private IEnumerator TimeOver()
    {
        yield return CoroutineHelper.WaitTime(0.1f);
        render.materials = monsterMaterial;
    }
}
