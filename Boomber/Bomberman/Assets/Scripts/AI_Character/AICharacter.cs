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

    [Header("����")]
    public MakeMap map;
    public GameObject bombPrefab;

    private float bombCooldown = 3f;
    private float bombTimer = 0f;

    private bool isInitialized = false;

    public void Initialize()
    {
        if (string.IsNullOrEmpty(characterId) || map == null || bombPrefab == null)
        {
            Debug.LogError($"[AICharacter] �ʱ�ȭ ����! �ʵ� ���� - characterId: {characterId}, map: {map}, bombPrefab: {bombPrefab}");
            return;
        }

        isInitialized = true;
        GameInstance.Instance.RegisterCharacter(characterId, new CharacterStat());
        Debug.Log($"[AICharacter] {characterId} �ʱ�ȭ �Ϸ�");
    }


    private void Update()
    {
        if (!isInitialized) return;

        moveTimer += Time.deltaTime;
        bombTimer += Time.deltaTime;
       // Debug.Log($"[AICharacter] ��ź Ÿ�̸�: {bombTimer}");

        //
        if (moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            TryMove(Vector2.right); // �׽�Ʈ ������ �̵�
        }

        if (bombTimer >= bombCooldown)
        {
            Debug.Log($"[AICharacter] ��ź ��ġ ���� �ϼ�. ĳ���� ��ġ: {transform.position}");
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
            Debug.Log($"[AICharacter] �̵� �Ұ�: {tileCoord} Ÿ���� ���̰ų� ��� ���Դϴ�.");
        }
    }

    private void PlaceBomb()
    {
        
        Vector3 placePos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), 0f);
        Debug.Log($"[AICharacter] ��ź ��ġ �õ�! ��ġ: {placePos}");


        GameObject bomb = Instantiate(bombPrefab, placePos, Quaternion.identity);

        if(bomb.TryGetComponent<Bomb>(out var bombscript))
        {
            bombscript.ownerCharacterId = characterId;
            Debug.Log($"[AICharacter] {characterId} �� ��ź ��ġ");
        }
        else
        {
            Debug.LogWarning("[AICharacter] Bomb ��ũ��Ʈ�� ã�� �� �����ϴ�.");
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
