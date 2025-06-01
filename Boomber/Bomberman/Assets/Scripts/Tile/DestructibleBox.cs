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


    ///<summary>
    ///�ܺο��� ȣ�� : ���ڰ� �ı��� �� ����
    ///</summary>
    ///

    public void DestroyBox()
    {
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

}
