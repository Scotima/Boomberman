using UnityEngine;
/// <summary>
/// 파괴 가능한 상자, 파괴 시 아이템이 드랍될 수 있음.
/// </summary>
/// 

public class DestructibleBox : MonoBehaviour
{
    [Header("파괴 시 드랍 가능한 아이템 프리맵")]
    public GameObject[] possibleItemPrefabs;

    [Header("아이템 드랍 확률 (0~1 사이)")]
    [Range(0f, 1f)] public float dropChance = 0.5f;

    public static event System.Action<Vector2Int> OnBoxDestroyed;


    ///<summary>
    ///외부에서 호출 : 상자가 파괴될 때 실행
    ///</summary>
    ///

    public void DestroyBox()
    {
        Vector2Int tile = new Vector2Int(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y)
        );

        GameInstance.Instance.RemoveBox(tile);
        Debug.Log($"[DestructibleBox] DestroyBox 호출됨 - tile: {tile}");

        GameInstance.Instance.map.SetTileEmpty(tile);


        TryDropItem();
        Destroy(gameObject);
    }

    ///<summary>
    ///아이템 드랍 시도.
    ///</summary>s
    ///

    private void TryDropItem()
    {
        if(possibleItemPrefabs.Length == 0)
        {
            return;
        }

        float rand = Random.value;

        if(rand <= dropChance)
        {
            int index = Random.Range(0, possibleItemPrefabs.Length);
            GameObject selectedItem = possibleItemPrefabs[index];
            Instantiate(selectedItem, transform.position, Quaternion.identity);
        }
    }

    ///<summary>
    ///폭발이나 공격 등에 의해 충돌 시 자동 파괴.
    /// </summary>
    /// 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Explosion"))
        {
            DestroyBox();
        }
    }

     private void OnDestroy()
    {
        Vector2Int tile = new Vector2Int(
            Mathf.RoundToInt(transform.position.x), // tileSize 정수라면 그대로 사용
            Mathf.RoundToInt(transform.position.y)
        );
        OnBoxDestroyed?.Invoke(tile);
        Debug.Log($"[DestructibleBox] 상자 파괴 → 타일 제거 요청: {tile}");
    }
}
