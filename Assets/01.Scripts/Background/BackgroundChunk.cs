using UnityEngine;

public enum BackgroundType
{
    None,
    Sky,
    GroundSurface,
    Underground
}

public class BackgroundChunk : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] public BackgroundType currentType;

    public Sprite skySprite;
    public Sprite groundSurfaceSprite;
    public Sprite undergroundSprite;

    public Vector2Int gridPosition { get; private set; }

    // ⭐ 렌더링 설정
    private BackgroundManager.SpriteRenderMode renderMode;
    private float chunkSize;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    // ⭐ 초기화 (설정 받기)
    public void Initialize(Vector2Int _gridPosition, float _chunkSize, BackgroundManager.SpriteRenderMode _renderMode)
    {
        gridPosition = _gridPosition;
        chunkSize = _chunkSize;
        renderMode = _renderMode;

        // ⭐ 월드 좌표 계산 (3단 구조에 맞게 조정)
        float worldY;

        if (gridPosition.y == 0)
        {
            // 땅 표면: Y = 0 (범위: -10 ~ 10)
            worldY = 0;
        }
        else if (gridPosition.y > 0)
        {
            // 하늘: Y = 10, 30, 50... (범위: 0~20, 20~40, 40~60...)
            worldY = gridPosition.y * chunkSize - chunkSize * 0.5f;
        }
        else // gridPosition.y < 0
        {
            // 지하: Y = -10, -30, -50... (범위: -20~0, -40~-20, -60~-40...)
            worldY = gridPosition.y * chunkSize + chunkSize * 0.5f;
        }

        Vector3 worldPos = new Vector3(
            gridPosition.x * chunkSize + chunkSize * 0.5f,  // X는 그대로
            worldY,  // ⭐ 계산된 Y
            0
        );
        transform.position = worldPos;

        // SortingOrder 설정
        if (gridPosition.y == 0)
        {
            spriteRenderer.sortingOrder = -99;
        }
        else
        {
            spriteRenderer.sortingOrder = -100;
        }

        // ⭐ SortingOrder 설정
        if (gridPosition.y == 0)
        {
            spriteRenderer.sortingOrder = -99;  // 땅 표면
        }
        else
        {
            spriteRenderer.sortingOrder = -100; // 하늘/땅속
        }

        UpdateBackgroundType();

        ApplyRenderMode();

    }

    // ⭐ 렌더링 모드 적용
    private void ApplyRenderMode()
    {
        switch (renderMode)
        {
            case BackgroundManager.SpriteRenderMode.AutoScale:
                SetupAutoScale();
                break;

            case BackgroundManager.SpriteRenderMode.Tiled:
                SetupTiled();
                break;
        }

        if (currentType == BackgroundType.GroundSurface)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.tileMode = SpriteTileMode.Continuous;
            spriteRenderer.size = new Vector2(20, 0.32f);
        }
    }

    private void SetupAutoScale()
    {
        Sprite refSprite = groundSurfaceSprite ?? skySprite ?? undergroundSprite;

        if (refSprite != null)
        {
            float spriteWidth = refSprite.bounds.size.x;
            float spriteHeight = refSprite.bounds.size.y;

            float scaleX = chunkSize / spriteWidth;
            float scaleY = chunkSize / spriteHeight;

            transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }


    private void SetupTiled()
    {
        // ⭐ sprite가 null이면 기본 스프라이트 생성
        if (spriteRenderer.sprite == null)
        {
            spriteRenderer.sprite = CreateDefaultSprite();
        }

        // ⭐ null 체크 후 Tiled 모드 설정
        if (spriteRenderer.sprite != null)
        {
            spriteRenderer.drawMode = SpriteDrawMode.Tiled;
            spriteRenderer.tileMode = SpriteTileMode.Continuous;
            spriteRenderer.size = new Vector2(chunkSize, chunkSize);
        }
        else
        {
            LogHelper.LogWarrning("Sprite가 null이어서 Tiled 모드를 설정할 수 없습니다.");
        }
    }

    public void UpdateBackgroundType()
    {
        BackgroundType newType;

        if (gridPosition.y > 0)
        {
            newType = BackgroundType.Sky;
        }
        else if (gridPosition.y == 0)
        {
            newType = BackgroundType.GroundSurface;
        }
        else
        {
            newType = BackgroundType.Underground;
        }

        if (currentType != newType)
        {
            currentType = newType;
            ApplySprite();
        }
    }

    private void ApplySprite()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Sprite targetSprite = null;
        Color targetColor = Color.white;

        switch (currentType)
        {
            case BackgroundType.Sky:
                targetSprite = skySprite;
                targetColor = new Color(0.6f, 0.8f, 1f, 1f);
                break;

            case BackgroundType.GroundSurface:
                targetSprite = groundSurfaceSprite;
                targetColor = Color.white;
                break;

            case BackgroundType.Underground:
                targetSprite = undergroundSprite;
                targetColor = new Color(0.5f, 0.35f, 0.25f, 1f);
                break;
        }

        // ⭐ sprite가 null이면 기본 스프라이트 생성
        if (targetSprite == null)
        {
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = CreateDefaultSprite();
            }
            LogHelper.LogWarrning($"{currentType} 스프라이트가 없어서 기본 스프라이트 사용");
        }
        else
        {
            spriteRenderer.sprite = targetSprite;
        }

        spriteRenderer.color = targetColor;
    }

    private Sprite CreateDefaultSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}