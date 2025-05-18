using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum MapSize
{
    Small,
    Medium,
    Large
}

public class MakeMap : MonoBehaviour
{
    [Header("맵 크기")]
    public MapSize selectedSize = MapSize.Medium;

    [Header("타일 프리맵")]
    public GameObject floorPrefab;
    public GameObject destructiblePrefab;
    public GameObject indestructiblePrefab;
    public float tileSize = 2.56f;

    [Header("타일 오프셋")]
    public Vector3 indestructibleOffset = Vector3.zero;
    public Vector3 destructibleOffset = Vector3.zero;

    private TileType[,] tileMap;
    private int mapWidth;
    private int mapHeight;
    private int indestructibleCount;
    private int playerCount;

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
                playerCount = 4;
                break;

            case MapSize.Medium:
                mapWidth = 23;
                mapHeight = 15;
                indestructibleCount = 77;
                playerCount = 8;
                break;
            case MapSize.Large:
                mapWidth = 31;
                mapHeight = 21;
                indestructibleCount = 150;
                playerCount = 8;
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
        // 전체 타일 수와 빈 공간 계산
        int totalTileCount = mapWidth * mapHeight;
        int emptySpace = totalTileCount - indestructibleCount;
        int minMovable = playerCount * 5;
        int placeable = emptySpace - minMovable;

        int min = Mathf.FloorToInt(placeable * 0.3f);
        int max = Mathf.FloorToInt(placeable * 0.7f);
        int toPlace = Random.Range(min, max + 1);
        int placed = 0;

        // ✅ 스폰 보호 좌표 설정
        HashSet<Vector2Int> reservedSpawnZones = new HashSet<Vector2Int>();
        Vector2Int[] spawnPositions = new Vector2Int[]
        {
        new Vector2Int(1, 1),
        new Vector2Int(mapWidth - 2, 1),
        new Vector2Int(1, mapHeight - 2),
        new Vector2Int(mapWidth - 2, mapHeight - 2)
        };

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

        // 상자 배치
        System.Random rng = new System.Random();

        while (placed < toPlace)
        {
            int x = rng.Next(1, mapWidth - 1);
            int y = rng.Next(1, mapHeight - 1);
            Vector2Int candidate = new Vector2Int(x, y);

            if (tileMap[x, y] == TileType.Empty && !reservedSpawnZones.Contains(candidate))
            {
                tileMap[x, y] = TileType.DestructibleWall;
                placed++;
            }
        }

        Debug.Log($"부서지는 상자 배치 완료: {placed}개 (범위: {min} ~ {max})");
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
                Debug.Log($"타일 {x},{y} → {basePos}");
                Debug.Log($"tileSize: {tileSize}");

                switch (tileMap[x, y])
                {
                    case TileType.IndestructibleWall:
                        Instantiate(indestructiblePrefab, basePos + indestructibleOffset, Quaternion.identity);
                        break;

                    case TileType.DestructibleWall:
                        Instantiate(destructiblePrefab, basePos + destructibleOffset, Quaternion.identity);
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

    //캐릭터가 해당 박스를 뚫고 지나가지 못하게 이동 가능한지 불가능한지 판단
    public bool IsWalkable(Vector2Int tileCoord)
    {
        if (tileCoord.x < 0 || tileCoord.x >= mapWidth || tileCoord.y < 0 || tileCoord.y >= mapHeight)
        {
            return false;
        }

        TileType tile = tileMap[tileCoord.x, tileCoord.y];
        return tile == TileType.Empty;
    }

}
