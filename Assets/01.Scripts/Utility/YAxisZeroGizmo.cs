using UnityEngine;

[ExecuteAlways]
public class YAxisZeroGizmo : MonoBehaviour
{
    public float gridSize = 10f;   // X,Z 범위
    public int gridLines = 20;     // 라인 개수
    public Color lineColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        float half = gridSize * 0.5f;
        float step = gridSize / gridLines;

        // Y=0 평면에 격자선 그리기
        for (float x = -half; x <= half; x += step)
            Gizmos.DrawLine(new Vector3(x, 0f, -half), new Vector3(x, 0f, half));

        for (float z = -half; z <= half; z += step)
            Gizmos.DrawLine(new Vector3(-half, 0f, z), new Vector3(half, 0f, z));

        // 중앙축 강조
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-half, 0f, 0f), new Vector3(half, 0f, 0f)); // X축
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(0f, 0f, -half), new Vector3(0f, 0f, half)); // Z축
    }
}
