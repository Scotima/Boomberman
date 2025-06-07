using UnityEngine;
/// <summary>
/// �ı� ������ ����, �ı� �� �������� ����� �� ����.
/// </summary>
/// 

public class DestructibleBox : MonoBehaviour
{
    [Header("�ı� �� ��� ������ ������ ������")]
    public GameObject[] possibleItemPrefabs;

    [Header("������ ��� Ȯ�� (0~1 ����)")]
    [Range(0f, 1f)] public float dropChance = 0.5f;

    public static event System.Action<Vector2Int> OnBoxDestroyed;


    ///<summary>
    ///�ܺο��� ȣ�� : ���ڰ� �ı��� �� ����
    ///</summary>
    ///

    public void DestroyBox()
    {
        Vector2Int tile = new Vector2Int(
        Mathf.RoundToInt(transform.position.x),
        Mathf.RoundToInt(transform.position.y)
        );

        GameInstance.Instance.RemoveBox(tile);
        Debug.Log($"[DestructibleBox] DestroyBox ȣ��� - tile: {tile}");

        GameInstance.Instance.map.SetTileEmpty(tile);


        TryDropItem();
        Destroy(gameObject);
    }

    ///<summary>
    ///������ ��� �õ�.
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
    ///�����̳� ���� � ���� �浹 �� �ڵ� �ı�.
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
            Mathf.RoundToInt(transform.position.x), // tileSize ������� �״�� ���
            Mathf.RoundToInt(transform.position.y)
        );
        OnBoxDestroyed?.Invoke(tile);
        Debug.Log($"[DestructibleBox] ���� �ı� �� Ÿ�� ���� ��û: {tile}");
    }
}
