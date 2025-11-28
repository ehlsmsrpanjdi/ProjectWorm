using System.Collections.Generic;
using UnityEngine;

public class ChaseWormDetector : MonoBehaviour
{
    public List<Edible> nearbyEdibles = new List<Edible>();

    [Header("설정")]
    public float cleanupInterval = 0.5f; // 청소 간격
    private float cleanupTimer = 0f;

    private void OnTriggerEnter2D(Collider2D col)
    {
        Edible e = col.GetComponent<Edible>();
        if (e != null && !nearbyEdibles.Contains(e))
        {
            nearbyEdibles.Add(e);
            LogHelper.Log($"ParasiteWormDetector: {e.name} 감지");
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        Edible e = col.GetComponent<Edible>();
        if (e != null && nearbyEdibles.Contains(e))
        {
            nearbyEdibles.Remove(e);
            LogHelper.Log($"ParasiteWormDetector: {e.name} 범위 벗어남");
        }
    }

    // ⭐ Update 추가: 주기적으로 청소
    private void Update()
    {
        cleanupTimer += Time.deltaTime;
        if (cleanupTimer >= cleanupInterval)
        {
            CleanupDeadEdibles();
            cleanupTimer = 0f;
        }
    }

    // ⭐ 죽은/null 몬스터 제거
    private void CleanupDeadEdibles()
    {
        // ⭐ 안전한 제거
        int removedCount = 0;

        for (int i = nearbyEdibles.Count - 1; i >= 0; i--)
        {
            Edible e = nearbyEdibles[i];

            try
            {
                if (e == null || e.gameObject == null || e.IsDead())
                {
                    nearbyEdibles.RemoveAt(i);
                    removedCount++;
                }
            }
            catch (MissingReferenceException)
            {
                // 파괴된 오브젝트
                nearbyEdibles.RemoveAt(i);
                removedCount++;
            }
        }

        if (removedCount > 0)
        {
            LogHelper.Log($"ParasiteWormDetector: {removedCount}개의 무효한 Edible 제거");
        }
    }

    public Edible GetNearestEdible(Transform self)
    {
        // ⭐ 먼저 청소
        CleanupDeadEdibles();

        Edible nearest = null;
        float minDist = float.MaxValue;

        // ⭐ ToArray()로 복사해서 안전하게 순회
        Edible[] ediblesArray = nearbyEdibles.ToArray();

        foreach (var e in ediblesArray)
        {
            // ⭐ 더 엄격한 체크
            if (e == null) continue;

            try
            {
                if (e.gameObject == null) continue;
                if (e.IsDead()) continue;

                float d = Vector2.Distance(self.position, e.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = e;
                }
            }
            catch (MissingReferenceException)
            {
                // 파괴된 오브젝트는 스킵
                continue;
            }
        }

        if (nearest != null)
        {
            LogHelper.Log($"가장 가까운 Edible: {nearest.name}, 거리: {minDist:F2}");
        }

        return nearest;
    }

    // ⭐ 디버그용
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // 감지 범위 표시
        Gizmos.color = Color.yellow;
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null)
        {
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }

        // 감지된 Edible 표시
        Gizmos.color = Color.red;
        foreach (var e in nearbyEdibles)
        {
            if (e != null && !e.IsDead())
            {
                Gizmos.DrawLine(transform.position, e.transform.position);
            }
        }
    }
}