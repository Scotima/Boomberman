using NUnit.Framework;
using System.Security.Cryptography;
using UnityEngine;
using System.Collections.Generic;

public class GameModeManager : MonoBehaviour
{
    [Header("�� ������")]
    public MakeMap mapGenerator;

    [Header("�÷��̾� ������")]
    public GameObject[] playerPrefabs;

    [Header("��ź ������")]
    public GameObject bombPrefab;

    private bool gameStarted = false;
    private List<Transform> generatedSpawnPoints = new List<Transform>();

    private void Start()
    {
        StartGame(); // ������
    }

    /// <summary>
    /// ������ �����մϴ�.
    /// </summary>
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;

        // �� ���� ����
        mapGenerator.CreateFullMap();

        // �÷��̾� ���� �� ũ�� ������ �����ϰ� ������
        int width = mapGenerator.GetMapWidth();
        int height = mapGenerator.GetMapHeight();
        int playerCount = mapGenerator.GetPlayerCount();

        GenerateSpawnPoints(width, height, playerCount);
        SpawnPlayers();
    }

    /// <summary>
    /// �÷��̾���� �����մϴ�.
    /// </summary>
    private void SpawnPlayers()
    {
        int count = Mathf.Min(playerPrefabs.Length, generatedSpawnPoints.Count);

        for (int i = 0; i < count; i++)
        {
            GameObject playerObj = Instantiate(playerPrefabs[i], generatedSpawnPoints[i].position, Quaternion.identity); // ���� ���� ĳ���� ��ȯ.
            string characterId = GameInstance.Instance.GenerateCharacterId("Player");

            var character = playerObj.GetComponent<AICharacter>();

            character.characterId = characterId;

            character.map = mapGenerator;
            character.bombPrefab = bombPrefab;

            character.Initialize();
            Debug.Log($"[GM] mapGenerator is null? {mapGenerator == null}");


            GameInstance.Instance.RegisterCharacter(characterId, new CharacterStat());
        }
    }

    /// <summary>
    /// �� ũ��� �÷��̾� ���� ���� ���� ��ġ �ڵ� ����
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

        Debug.Log($"[GameModeManager] ���� ��ġ ���� �Ϸ�: {generatedSpawnPoints.Count}��");
    }

    public void EndGame()
    {
        gameStarted = false;
        Debug.Log("���� ����");
    }

    public void NotifyBlockDestroyed(Vector2Int pos)
    {
        Debug.Log($"�� �ı���: {pos}");
    }
}
