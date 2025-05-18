using NUnit.Framework;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    [Header("맵 생성기")]
    public MakeMap mapGenerator;

    [Header("플레이어 프리팹")]
    public GameObject[] playerPrefabs;

    private bool gameStarted = false;
    private List<Transform> generatedSpawnPoints = new List<Transform>();

    private void Start()
    {
        StartGame(); // 진입점
    }

    /// <summary>
    /// 게임을 시작합니다.
    /// </summary>
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        // 맵 생성 위임
        mapGenerator.CreateFullMap();

        // 플레이어 수와 맵 크기 정보를 안전하게 가져옴
        int width = mapGenerator.GetMapWidth();
        int height = mapGenerator.GetMapHeight();
        int playerCount = mapGenerator.GetPlayerCount();

        GenerateSpawnPoints(width, height, playerCount);
        SpawnPlayers();
    }

    /// <summary>
    /// 플레이어들을 스폰합니다.
    /// </summary>
    private void SpawnPlayers()
    {
        int count = Mathf.Min(playerPrefabs.Length, generatedSpawnPoints.Count);

        for (int i = 0; i < count; i++)
        {
            Instantiate(playerPrefabs[i], generatedSpawnPoints[i].position, Quaternion.identity);
        }
    }

    /// <summary>
    /// 맵 크기와 플레이어 수에 따라 스폰 위치 자동 생성
    /// </summary>
    private void GenerateSpawnPoints(int mapWidth, int mapHeight, int playerCount)
    {
        generatedSpawnPoints.Clear();

        Vector3[] basePositions;

        if (playerCount <= 4)
        {
            basePositions = new Vector3[]
            {
                new Vector3(1, 1, 0),
                new Vector3(mapWidth - 2, 1, 0),
                new Vector3(1, mapHeight - 2, 0),
                new Vector3(mapWidth - 2, mapHeight - 2, 0)
            };
        }
        else
        {
            basePositions = new Vector3[]
            {
                new Vector3(1, 1, 0), new Vector3(mapWidth - 2, 1, 0),
                new Vector3(1, mapHeight - 2, 0), new Vector3(mapWidth - 2, mapHeight - 2, 0),
                new Vector3(mapWidth / 2, 1, 0), new Vector3(mapWidth / 2, mapHeight - 2, 0),
                new Vector3(1, mapHeight / 2, 0), new Vector3(mapWidth - 2, mapHeight / 2, 0)
            };
        }

        float tileSize = mapGenerator.tileSize;
        for (int i = 0; i < playerCount && i < basePositions.Length; i++)
        {
            Vector3 worldPos = basePositions[i] * tileSize;
            GameObject spawnPointObj = new GameObject($"AutoSpawnPoint_{i}");
            spawnPointObj.transform.position = worldPos;
            generatedSpawnPoints.Add(spawnPointObj.transform);
        }

        Debug.Log($"[GameModeManager] 스폰 위치 생성 완료: {generatedSpawnPoints.Count}개");
    }

    public void EndGame()
    {
        gameStarted = false;
        Debug.Log("게임 종료");
    }

    public void NotifyBlockDestroyed(Vector2Int pos)
    {
        Debug.Log($"벽 파괴됨: {pos}");
    }
}
