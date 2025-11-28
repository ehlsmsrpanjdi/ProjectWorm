using UnityEngine;

public class ChaseWorm : MonoBehaviour
{
    [Header("컴포넌트")]
    public ChaseWormAI wormAi;
    public ChaseWormDetector detector;

    private void Awake()
    {
        // ⭐ AI는 자식에 있음
        wormAi = GetComponentInChildren<ChaseWormAI>();
        detector = GetComponentInChildren<ChaseWormDetector>();

        if (wormAi == null)
        {
            LogHelper.LogError("ChaseWormAI를 찾을 수 없습니다!");
        }

    }

    private void Update()
    {

    }
}