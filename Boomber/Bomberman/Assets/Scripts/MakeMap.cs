using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;



public class MakeMap : MonoBehaviour
{
    public enum MapSize
    {
        Small,
        Medium,
        Large
    }

    [Header("맵 크기")]
    public MapSize selectedSize = MapSize.Medium;

    [Header("타일 프리맵")]
    public GameObject floorPrefab;
    public GameObject destructiblePrefab;
    public GameObject indestructiblePrefab;
    public float tileSize = 1f;

    [Header("타일 오프셋")]
    public Vector3 indestructibleOffset = Vector3.zero;
    public Vector3 destructibleOffset = Vector3.zero;

    private TileType[,] tileMap;
    private int mapWidth;
    private int mapHeight;
    private int indestructibleCount;
    public int playerCount;

    public Transform destructibleBoxParent;

    private List<Vector2Int> destructibleBoxTiles = new();

    public enum TileType
    { 
        Empty,
        IndestructibleWall, // 파괴할 수 없는 벽
        DestructibleWall // 파괴 가능한 벽
    }

    //private void Start()
    //{
    //    ApplyMapConfig();
    //    GenerateMap();
    //    PrintMapToConsole(); // 테스트 콘솔 출력
    //    RenderMap(); // 실제 오브젝트 배치

    //}

    private void ApplyMapConfig()
    {
        switch (selectedSize)
        {
            case MapSize.Small:
                mapWidth = 15;
                mapHeight = 9;
                indestructibleCount = 28;
                //playerCount = 4;
                break;

            case MapSize.Medium:
                mapWidth = 23;
                mapHeight = 15;
                indestructibleCount = 77;
                //playerCount = 8;
                break;
            case MapSize.Large:
                mapWidth = 31;
                mapHeight = 21;
                indestructibleCount = 150;
               //playerCount = 8;
                break;

        }

    }

   private void GenerateMap()
    {
        tileMap = new TileType[mapWidth, mapHeight];
        PlaceWalls();
        PlaceDestructibles();
    }

    void PlaceWalls()
    {
        for(int x = 0; x < mapWidth; x ++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                if(x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1 || (x % 2 == 0 && y % 2 == 0)) // 부서지지 않는 벽 생성 맵 크기가 될것
                {
                    tileMap[x, y] = TileType.IndestructibleWall;
                }

                else
                {
                    tileMap[x, y] = TileType.Empty; // 나머지 빈공간.
                }
            }
        }
    }

    void PlaceDestructibles()
    {
        int totalTileCount = mapWidth * mapHeight;
        int emptySpace = totalTileCount - indestructibleCount;
        int minMovable = playerCount * 5;
        int placeable = emptySpace - minMovable;

        int min = Mathf.FloorToInt(placeable * 0.3f);
        int max = Mathf.FloorToInt(placeable * 0.7f);
        int toPlace = Random.Range(min, max + 1);
        int placed = 0;

        // ✅ 1. 스폰 지점 생성 (8인까지 고려)
        List<Vector2Int> spawnPositions = new List<Vector2Int>();
        if (playerCount >= 1) spawnPositions.Add(new Vector2Int(1, 1));
        if (playerCount >= 2) spawnPositions.Add(new Vector2Int(mapWidth - 2, 1));
        if (playerCount >= 3) spawnPositions.Add(new Vector2Int(1, mapHeight - 2));
        if (playerCount >= 4) spawnPositions.Add(new Vector2Int(mapWidth - 2, mapHeight - 2));
        if (playerCount >= 5) spawnPositions.Add(new Vector2Int(mapWidth / 2, 1));
        if (playerCount >= 6) spawnPositions.Add(new Vector2Int(mapWidth / 2, mapHeight - 2));
        if (playerCount >= 7) spawnPositions.Add(new Vector2Int(1, mapHeight / 2));
        if (playerCount >= 8) spawnPositions.Add(new Vector2Int(mapWidth - 2, mapHeight / 2));

        // ✅ 2. 스폰 보호 영역 설정
        HashSet<Vector2Int> reservedSpawnZones = new HashSet<Vector2Int>();
        foreach (var pos in spawnPositions)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int x = pos.x + dx;
                    int y = pos.y + dy;

                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                    {
                        reservedSpawnZones.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        // ✅ 3. 상자 배치 (무한 루프 방지 추가)
        System.Random rng = new System.Random();
        int maxTries = 10000;
        int tries = 0;

        while (placed < toPlace && tries < maxTries)
        {
            tries++;

            int x = rng.Next(1, mapWidth - 1);
            int y = rng.Next(1, mapHeight - 1);
            Vector2Int candidate = new Vector2Int(x, y);

            if (tileMap[x, y] == TileType.Empty && !reservedSpawnZones.Contains(candidate))
            {
                tileMap[x, y] = TileType.DestructibleWall;
                placed++;
            }
        }

        Debug.Log($"[상자 배치] 총 {placed}개 배치 완료 (범위: {min} ~ {max})");
        if (tries >= maxTries)
            Debug.LogWarning($"[상자 배치] 최대 시도 횟수 초과로 중단됨. placed={placed}, target={toPlace}");
    }

    void PrintMapToConsole()
    {
        for(int y = mapHeight - 1; y >= 0; y--)
        {
            string row = "";
            for(int x = mapWidth - 1; x >= 0; x--)
            {
                switch(tileMap[x,y])
                {


                    case TileType.IndestructibleWall: 
                        row += "■";
                        break;
                    case TileType.DestructibleWall:
                        row += "□";
                        break;
                    default:
                        row += " ";
                        break;
                }
            }
            Debug.Log(row);
        }
    }

   private void RenderMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 basePos = new Vector3(x * tileSize , y * tileSize, 0f);
                Instantiate(floorPrefab, basePos, Quaternion.identity);
                //Debug.Log($"타일 {x},{y} → {basePos}");
               // Debug.Log($"tileSize: {tileSize}");

                switch (tileMap[x, y])
                {
                    case TileType.IndestructibleWall:
                        Instantiate(indestructiblePrefab, basePos + indestructibleOffset, Quaternion.identity);
                        break;

                    case TileType.DestructibleWall:
                        Instantiate(destructiblePrefab, basePos + destructibleOffset, Quaternion.identity);
                        Vector2Int tile = new Vector2Int(x, y);
                        RegisterBox(tile);
                        break;
                }
            }
        }
    }

    public void CreateFullMap()
    {
        ApplyMapConfig();
        GenerateMap();
        RenderMap();

        // 📷 카메라 중앙 정렬
        Vector3 centerPosition = new Vector3(
            (mapWidth - 1) * tileSize / 2f,
            (mapHeight - 1) * tileSize / 2f,
            -10f
        );
        Camera.main.transform.position = centerPosition;
    }


    public int GetMapWidth()
    {
        return mapWidth;
    }

    public int GetMapHeight()
    {
        return mapHeight;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }

    /// <summary>
    /// 타일 좌표(tile)를 월드 좌표로 변환해, 반경 radius 내에 DestructibleBox가 있는지 검사.
    /// </summary>
    /// 

    public bool IsDestructibleBoxVirtual(Vector2Int tile, float radius, AICharacter debugGizmoTarget = null)
    {
        // 1) 타일 중앙을 월드 좌표로 계산
        Vector2 worldPos = new Vector2(tile.x * tileSize, tile.y * tileSize);

        if (debugGizmoTarget != null)
        {
            debugGizmoTarget.debugGizmoPos = worldPos;
            debugGizmoTarget.debugGizmoRadius = radius;
        }

        // 2) 주어진 반경(radius) 내 모든 Collider2D를 검색
        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, radius);

       // Debug.Log($"[감지] {tile} → hits.Length = {hits.Length}");
        Debug.Log($"[DFS] 타일 {tile} → worldPos: {worldPos}, hits: {hits.Length}");

        // 3) 그중 DestructibleBox 컴포넌트를 가진 오브젝트가 있으면 true
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<DestructibleBox>(out var box))
            {
                if (!box.gameObject.activeInHierarchy) continue; // 이미 비활성화된 상자 무시
                return true;
            }
        }

        return false;
    }

    //캐릭터가 해당 박스를 뚫고 지나가지 못하게 이동 가능한지 불가능한지 판단
    public bool IsWalkable(Vector2Int tileCoord, bool allowDestructible = false)
    {
        if (tileCoord.x < 0 || tileCoord.x >= mapWidth || tileCoord.y < 0 || tileCoord.y >= mapHeight)
            return false;

        TileType tile = tileMap[tileCoord.x, tileCoord.y];

        Debug.Log($"[IsWalkable] 검사 tile: {tileCoord} → {tileMap[tileCoord.x, tileCoord.y]}");

        bool walkable = tile == TileType.Empty || (allowDestructible && tile == TileType.DestructibleWall);

        if (!walkable)
            Debug.Log($"[IsWalkable] 이동 불가 → {tileCoord} 타일은 {tile}");

        return walkable;
    }

    /// <summary>
    /// 지정한 타일 좌표(tile)가 파괴 가능한 상자가 있는 위치인지 여부를 반환.
    /// - Physics2D.OverlapCircleAll을 사용해서, 해당 타일 중앙 근처에
    ///   DestructibleBox 컴포넌트가 붙은 게임 오브젝트가 있는지 검사한다.
    /// </summary>
    /// <param name="tile">타일 좌표 (예: Vector2Int(x, y))</param>
    /// <returns>해당 좌표에 DestructibleBox가 하나라도 있으면 true, 아니면 false</returns>
    /// 

    public bool IsDestructibleBox(Vector2Int tile)
    {
        // 1) 타일 좌표를 월드 좌표로 변환
        //    - 각 타일의 중앙 위치를 찾아야 하므로, tile.x * tileSize, tile.y * tileSize 형태로 계산
        Vector2 worldPos = new Vector2(tile.x * tileSize, tile.y * tileSize);
        Debug.Log($"[IsDestructibleBox] 검사 tile={tile} → worldPos={worldPos}");

        // 2) Physics2D.OverlapCircleAll을 사용해, 
        //    해당 worldPos 근처(작은 반경)에서 충돌체(Colliders)를 모두 찾아온다.
        //    반경 값(0.1f)은 타일 중앙 근처만 검사하기 위한 작은 값이므로, 상황에 맞게 조정 가능.

        Collider2D[] hits = Physics2D.OverlapCircleAll(worldPos, tileSize * 0.5f);
        Debug.Log($"[IsDestructibleBox] OverlapCircleAll 반경={tileSize * 0.4f} → hits.Length={hits.Length}");

        foreach (var hit in hits)
        {
            if(hit.TryGetComponent<DestructibleBox>(out var box))
            {
                Debug.Log($"[IsDestructibleBox] 상자 감지: {hit.name} @ {hit.transform.position}");
                return true;
            }
        }
        return false;
    }

    public void RegisterBox(Vector2Int tile)
    {
        GameInstance.Instance.RegisterBox(tile);
        //Debug.Log($"[MakeMap] 상자 등록됨: {tile}");
    }



   public List<Vector2Int> GetAllDestructibleBoxTiles()
   {
       return new List<Vector2Int>(GameInstance.Instance.destructibleBoxTiles);
   }

    public void SetTileEmpty(Vector2Int tile)
    {
        tileMap[tile.x, tile.y] = TileType.Empty;
        Debug.Log($"[MakeMap] tileMap[{tile.x},{tile.y}] → Empty 설정됨");
    }
}


