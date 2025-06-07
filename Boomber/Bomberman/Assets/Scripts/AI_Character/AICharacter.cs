using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.PlayerSettings;



public class AICharacter : MonoBehaviour
{

    [Header("UI")]
    public GameObject statUIPrefab;
    private StatUI statUI;

    public float moveSpeed = 2f;
    public float moveInterval = 1f;
    private float moveTimer = 0f;

    public string characterId;

    private VirtualDFSSearch search;

    [SerializeField] private CharacterStat stat;
    public CharacterStat GetStat() => stat;

    [Header("참조")]
    public MakeMap map;
    public GameObject bombPrefab;

    [Header("상자 탐색 반경")]
    public float searchRadius = 1f;

    private List<Vector2Int> currentPath = null;
    private int pathIndex = 0; // 현재 경로에서 몇 번째 칸에 있는지
    public int maxDepth = 200;

    private float bombCooldown = 1f;
    private float bombTimer = 0f;

    private bool isInitialized = false;

    public Vector2 debugGizmoPos;
    public float debugGizmoRadius = 0f;

    Vector2Int? targetBoxTile = null;



    public void Initialize()
    {
        if (string.IsNullOrEmpty(characterId) || map == null || bombPrefab == null)
        {
            Debug.LogError($"[AICharacter] 초기화 실패! 필드 누락 - characterId: {characterId}, map: {map}, bombPrefab: {bombPrefab}");
            return;
        }

        isInitialized = true;
        Debug.Log($"[AICharacter] {characterId} 초기화 완료");


        GameObject uiObj = Instantiate(statUIPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        uiObj.transform.SetParent(transform); // 캐릭터에 붙임
        statUI = uiObj.GetComponent<StatUI>();
        statUI.UpdateUI(GameInstance.Instance.GetCharacterStat(characterId));

        StartCoroutine(AILoop());
    }




    void TryMove(Vector2 direction)
    {
        Vector3 nextPos = transform.position + (Vector3)(direction * map.tileSize);
        Vector2Int nextTile = new Vector2Int(
            Mathf.FloorToInt(nextPos.x / map.tileSize),
            Mathf.FloorToInt(nextPos.y / map.tileSize)
        );

        if (map.IsWalkable(nextTile))
            transform.position = nextPos;
    }

    private void PlaceBomb()
    {

        Vector3 placePos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
        Debug.Log($"[AICharacter] 폭탄 설치 시도! 위치: {placePos}");


        GameObject bomb = Instantiate(bombPrefab, placePos, Quaternion.identity);

        if (bomb.TryGetComponent<Bomb>(out var bombscript))
        {
            bombscript.ownerCharacterId = characterId;
            Debug.Log($"[AICharacter] {characterId} 가 폭탄 설치");
        }
        else
        {
            Debug.LogWarning("[AICharacter] Bomb 스크립트를 찾을 수 없습니다.");
        }

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Item>(out var item))
        {
            GameInstance.Instance.ApplyItemToCharacter(characterId, item);
            Destroy(other.gameObject);
        }
    }



    private void MoveOneStepTowards(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        Vector2 dir = Mathf.Abs(diff.x) > Mathf.Abs(diff.y)
            ? (diff.x > 0 ? Vector2.right : Vector2.left)
            : (diff.y > 0 ? Vector2.up : Vector2.down);

        Vector3 nextPos = transform.position + (Vector3)(dir * map.tileSize);
        Vector2Int nextTile = new Vector2Int(
            Mathf.FloorToInt(nextPos.x / map.tileSize),
            Mathf.FloorToInt(nextPos.y / map.tileSize)
        );

        if (map.IsWalkable(nextTile))
        {
            Debug.Log($"[이동] {from} → {nextTile} 방향: {dir}");
            transform.position = nextPos;
        }
        else
        {
            Debug.LogWarning($"[이동 실패] {nextTile} 은 이동 불가 (Walkable false)");
        }
    }

    public void UpdateStat(CharacterStat newStat)
    {
        stat = newStat;

        moveSpeed = stat.GetMoveSpeed();

        // 이동 주기를 이동 속도 기반으로 다시 계산
        moveInterval = 1f / moveSpeed;

        if (statUI != null)
            statUI.UpdateUI(stat);
        Debug.LogWarning($"[AICharacter] {characterId} 능력치 갱신됨 → 속도: {moveSpeed}, 파워: {stat.GetBombPower()}, 개수: {stat.GetBombCount()}");
    }

    private Vector2Int GetMyTile()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x / map.tileSize),
            Mathf.RoundToInt(transform.position.y / map.tileSize)
        );
    }

    private IEnumerator AILoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (!isInitialized || stat.GetIsDead())
                yield break;

            Vector2Int myTile = GetMyTile();
            List<Vector2Int> candidates = map.GetAllDestructibleBoxTiles();
            List<AICharacter> enemies = GameInstance.Instance.GetLivingEnemies(characterId);
            AICharacter nearestEnemy = null;
            float minDist = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float dist = Vector2Int.Distance(myTile, enemy.GetMyTile());
                if (dist < minDist)
                {
                    nearestEnemy = enemy;
                    minDist = dist;
                }
            }

            // ✅ 경로 없을 때만 DFS 실행
            if (currentPath == null || currentPath.Count == 0)
            {
                bool foundPath = false;

                // 🎯 우선순위 1: 상자 존재
                if (candidates.Count > 0)
                {
                    candidates.Sort((a, b) => Vector2Int.Distance(myTile, a).CompareTo(Vector2Int.Distance(myTile, b)));

                    foreach (var boxTile in candidates)
                    {
                        List<Vector2Int> neighbors = GetWalkableNeighbors(boxTile);
                        foreach (var neighbor in neighbors)
                        {
                            var search = new VirtualDFSSearch(map, myTile, neighbor, searchRadius, maxDepth);
                            while (!search.IsFinished && !search.FoundBox)
                                search.Step();

                            if (search.FoundBox)
                            {
                                currentPath = search.Path;
                                pathIndex = 0;
                                targetBoxTile = boxTile;
                                foundPath = true;
                                break;
                            }
                        }

                        if (foundPath) break;
                    }
                }
                // 🎯 우선순위 2: 적이 가까이 있을 때 추적
                else if (nearestEnemy != null && minDist <= 5f)
                {
                    List<Vector2Int> neighbors = GetWalkableNeighbors(nearestEnemy.GetMyTile());
                    foreach (var neighbor in neighbors)
                    {
                        var search = new VirtualDFSSearch(map, myTile, neighbor, searchRadius, maxDepth);
                        Debug.LogWarning($"[AI] 적 {nearestEnemy.characterId} 추적 DFS 시작 → 시작: {myTile}, 목표: {neighbor}");
                        while (!search.IsFinished && !search.FoundBox)
                            search.Step();

                        if (search.FoundBox)
                        {
                            currentPath = search.Path;
                            pathIndex = 0;
                            targetBoxTile = null; // 적 추적일 땐 null
                            foundPath = true;
                            Debug.LogWarning($"[AI] 적 {nearestEnemy.characterId} 경로 확보 완료! 경로 길이: {currentPath.Count}, 목표 타일: {neighbor}");
                            break;
                        }

                        else
                        {
                            Debug.LogWarning($"[AI] 적 {nearestEnemy.characterId} 경로 탐색 실패 → 목표: {neighbor}");
                        }
                    }
                }

                // 🔚 아무것도 못 찾았으면 종료
                if (!foundPath)
                {
                    Debug.Log("[AI] 상자도 없고 적도 없어 경로 없음 → 루프 종료");
                    yield break;
                }
            }

            // ✅ 경로 따라 이동
            while (currentPath != null && pathIndex < currentPath.Count)
            {
                Vector2Int nextTile = currentPath[pathIndex];
                Vector2 nextWorldPos = (Vector2)nextTile * map.tileSize;

                if (Vector2.Distance(transform.position, nextWorldPos) < 0.05f)
                {
                    pathIndex++;

                    if (pathIndex >= currentPath.Count && targetBoxTile.HasValue &&
                        Vector2Int.Distance(GetMyTile(), targetBoxTile.Value) <= 1f)
                    {
                        Debug.Log($"[AI] 목표 상자 {targetBoxTile.Value} 근처 도달 → 폭탄 설치");
                        PlaceBomb();
                        yield return new WaitForSeconds(1f);
                        yield return null;

                        currentPath = null;
                        pathIndex = 0;
                        targetBoxTile = null;
                        break;
                    }

                    continue;
                }

                MoveOneStepTowards(GetMyTile(), nextTile);
                yield return new WaitForSeconds(moveInterval);
            }

            // ✅ 경로 종료 정리
            if (pathIndex >= currentPath?.Count)
            {
                currentPath = null;
                pathIndex = 0;
                targetBoxTile = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debugGizmoRadius > 0f)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(debugGizmoPos, debugGizmoRadius);
        }


    }

    private List<Vector2Int> GetWalkableNeighbors(Vector2Int center)
    {
        List<Vector2Int> results = new();

        Vector2Int[] dirs = new Vector2Int[]
        {
             Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in dirs)
        {
            Vector2Int neighbor = center + dir;
            Debug.Log($"[AI] 상자 {center} 주변 검사 중 → neighbor: {neighbor}");
            if (map.IsWalkable(neighbor, false)) // 상자 못 지나감
                results.Add(neighbor);
            else
            {
                Debug.Log($"[AI] {neighbor} 는 걷기 불가 (상자 주위지만 막힘)");
            }
        }

        return results;
    }

    //데미지 기능 함수

    public void TakeDamage(int amount)
    {
        stat.ApplyDamage(amount);
        Debug.LogWarning($"[AICharacter] {characterId} 피격! 남은 체력: {stat.GetCurrentHP()}");

        if(stat.GetIsDead())
        {
            Debug.LogWarning($"[AICharacter] {characterId} 사망 처리");
            Die();
            
        }
    }
    private void Die()
    {
        // 임시: 오브젝트 제거
        StopAllCoroutines();
        Destroy(gameObject);
    }
   


}
