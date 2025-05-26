using UnityEngine;
using UnityEngine.TextCore.Text;

public class AICharacter : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float moveInterval = 1f;
    private float moveTimer = 0f;

    public string characterId;

    private CharacterStat stat;

    [Header("참조")]
    public MakeMap map;

    private void Update()
    {
        moveTimer += Time.deltaTime;

        //
        if(moveTimer >= moveInterval)
        {
            moveTimer = 0f;
            TryMove(Vector2.right); // 테스트 오른쪽 이동
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
