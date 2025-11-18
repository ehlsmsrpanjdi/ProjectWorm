using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    private static BackgroundManager instance;
    public static BackgroundManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("BackgroundManager");
                instance = go.AddComponent<BackgroundManager>();
            }
            return instance;
        }
    }

    [Header("설정")]
    [SerializeField] private float chunkSize = 20f;
    [SerializeField] private int viewDistance = 2;

    [Header("스프라이트")]
    [SerializeField] private Sprite skySprite;
    [SerializeField] private Sprite groundSurfaceSprite;  // ⭐ 추가!
    [SerializeField] private Sprite undergroundSprite;

    [Header("렌더링 모드")]
    [SerializeField] private SpriteRenderMode renderMode = SpriteRenderMode.Tiled;

    public enum SpriteRenderMode
    {
        AutoScale,
        Tiled
    }

    private Transform cameraTransform;
    private Vector2Int currentCameraChunk;
    private Dictionary<Vector2Int, BackgroundChunk> activeChunks = new Dictionary<Vector2Int, BackgroundChunk>();
    private Queue<BackgroundChunk> chunkPool = new Queue<BackgroundChunk>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // ⭐ Awake에서는 스프라이트만 로드
        LoadSprites();
    }

    private void LoadSprites()
    {
        if (skySprite == null)
        {
            skySprite = Resources.Load<Sprite>("Background/SkyBackground");
        }

        if (groundSurfaceSprite == null)
        {
            groundSurfaceSprite = Resources.Load<Sprite>("Background/GroundSurface");
        }

        if (undergroundSprite == null)
        {
            undergroundSprite = Resources.Load<Sprite>("Background/UndergroundBackground");
        }

        if (skySprite == null || groundSurfaceSprite == null || undergroundSprite == null)
        {
            LogHelper.LogWarrning("일부 배경 스프라이트를 찾을 수 없습니다. 기본 색상 사용");
        }

        cameraTransform = Camera.main.transform;

        CreateInitialChunks();
    }

    private void CreateInitialChunks()
    {
        currentCameraChunk = WorldToChunk(cameraTransform.position);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkPos = currentCameraChunk + new Vector2Int(x, y);
                CreateChunk(chunkPos);
            }
        }

        LogHelper.Log($"배경 초기화 완료: {activeChunks.Count}개 청크");
    }

    private void Update()
    {
        Vector2Int newCameraChunk = WorldToChunk(cameraTransform.position);

        if (newCameraChunk != currentCameraChunk)
        {
            UpdateChunks(newCameraChunk);
            currentCameraChunk = newCameraChunk;
        }
    }

    private Vector2Int WorldToChunk(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / chunkSize),
            Mathf.FloorToInt(worldPos.y / chunkSize)
        );
    }

    private void UpdateChunks(Vector2Int newCenterChunk)
    {
        HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkPos = newCenterChunk + new Vector2Int(x, y);
                requiredChunks.Add(chunkPos);
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!requiredChunks.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var pos in toRemove)
        {
            RecycleChunk(pos);
        }

        foreach (var pos in requiredChunks)
        {
            if (!activeChunks.ContainsKey(pos))
            {
                CreateChunk(pos);
            }
        }
    }

    private void CreateChunk(Vector2Int gridPos)
    {
        BackgroundChunk chunk;

        if (chunkPool.Count > 0)
        {
            chunk = chunkPool.Dequeue();
            chunk.gameObject.SetActive(true);
        }
        else
        {
            GameObject chunkObj = new GameObject($"Chunk_{gridPos.x}_{gridPos.y}");
            chunkObj.transform.SetParent(transform);

            // ⭐ BackgroundChunk만 추가 (Awake에서 SpriteRenderer 자동 생성)
            chunk = chunkObj.AddComponent<BackgroundChunk>();

            // 스프라이트 전달
            chunk.skySprite = skySprite;
            chunk.groundSurfaceSprite = groundSurfaceSprite;
            chunk.undergroundSprite = undergroundSprite;

            // ⭐ SpriteRenderer 추가 제거! (중복 방지)
        }

        // ⭐ 렌더링 모드 전달
        chunk.Initialize(gridPos, chunkSize, renderMode);
        activeChunks[gridPos] = chunk;
    }

    private void SetupAutoScale(GameObject chunkObj, SpriteRenderer sr)
    {
        // 기준 스프라이트로 스케일 계산 (groundSurface 우선)
        Sprite refSprite = groundSurfaceSprite ?? skySprite ?? undergroundSprite;

        if (refSprite != null)
        {
            float spriteWidth = refSprite.bounds.size.x;
            float spriteHeight = refSprite.bounds.size.y;

            float scaleX = chunkSize / spriteWidth;
            float scaleY = chunkSize / spriteHeight;

            chunkObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }

    private void SetupTiled(SpriteRenderer sr)
    {
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.tileMode = SpriteTileMode.Continuous;
        sr.size = new Vector2(chunkSize, chunkSize);
    }

    private void RecycleChunk(Vector2Int gridPos)
    {
        if (activeChunks.TryGetValue(gridPos, out BackgroundChunk chunk))
        {
            chunk.gameObject.SetActive(false);
            chunkPool.Enqueue(chunk);
            activeChunks.Remove(gridPos);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || cameraTransform == null) return;

        // 현재 카메라 청크
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            currentCameraChunk.x * chunkSize + chunkSize * 0.5f,
            currentCameraChunk.y * chunkSize + chunkSize * 0.5f,
            0
        );
        Gizmos.DrawWireCube(center, new Vector3(chunkSize, chunkSize, 0));

        // Y = 0 라인 (땅 표면)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0));

        // Y = chunkSize, -chunkSize 경계선
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-1000, chunkSize, 0), new Vector3(1000, chunkSize, 0));
        Gizmos.DrawLine(new Vector3(-1000, -chunkSize, 0), new Vector3(1000, -chunkSize, 0));
    }
}
