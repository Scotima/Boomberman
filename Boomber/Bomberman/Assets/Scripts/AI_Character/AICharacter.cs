using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.PlayerSettings;

public class AICharacter : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveInterval = 1f;
    private float moveTimer = 0f;

    public string characterId;

    private CharacterStat stat;

    [Header("참조")]
    public MakeMap map;
    public GameObject bombPrefab;

    private float bombCooldown = 3f;
    private float bombTimer = 0f;

    private bool isInitialized = false;

    public void Initialize()
    {
        if (string.IsNullOrEmpty(characterId) || map == null || bombPrefab == null)
        {
            Debug.LogError($"[AICharacter] 초기화 실패! 필드 누락 - characterId: {characterId}, map: {map}, bombPrefab: {bombPrefab}");
            return;
        }

        isInitialized = true;
        GameInstance.Instance.RegisterCharacter(characterId, new CharacterStat());
        Debug.Log($"[AICharacter] {characterId} 초기화 완료");
    }


    private void Update()
    {
        if (!isInitialized) return;

        moveTimer += Time.deltaTime;
        bombTimer += Time.deltaTime;
       // Debug.Log($"[AICharacter] 폭탄 타이머: {bombTimer}");

        //
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            TryMove(Vector2.right); // 테스트 오른쪽 이동
        }

        if (bombTimer >= bombCooldown)
        {
            Debug.Log($"[AICharacter] 폭탄 설치 조건 완성. 캐릭터 위치: {transform.position}");
            bombTimer = 0f;
            PlaceBomb();
        }
    }

   void TryMove(Vector2 direction)
    {
        Vector3 nextPos = transform.position + (Vector3)(direction * map.tileSize);

        Vector2Int tileCoord = new Vector2Int(Mathf.FloorToInt(nextPos.x / map.tileSize), Mathf.FloorToInt(nextPos.y / map.tileSize));

        if(map.IsWalkable(tileCoord))
        {
            transform.position = nextPos;

        }
        else
        {
            Debug.Log($"[AICharacter] 이동 불가: {tileCoord} 타일은 벽이거나 경계 밖입니다.");
        }
    }

    private void PlaceBomb()
    {
        
        Vector3 placePos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
        Debug.Log($"[AICharacter] 폭탄 설치 시도! 위치: {placePos}");


        GameObject bomb = Instantiate(bombPrefab, placePos, Quaternion.identity);

        if(bomb.TryGetComponent<Bomb>(out var bombscript))
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

    public void UpdateStat(CharacterStat newStat)
    {
        stat = newStat;
    }
}
