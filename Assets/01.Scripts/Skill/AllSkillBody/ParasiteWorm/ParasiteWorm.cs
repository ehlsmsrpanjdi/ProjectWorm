using UnityEngine;

public class ParasiteWorm : MonoBehaviour
{
    ParasiteWormAI wormAi;
    ParasiteWormDetector detector;

    private void Awake()
    {
        wormAi = GetComponentInChildren<ParasiteWormAI>();
        detector = GetComponentInChildren<ParasiteWormDetector>();
    }
}
