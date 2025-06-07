using UnityEngine;

public class Bomb : MonoBehaviour
{
    public string ownerCharacterId;
    public float explodeDelay = 2f;
    public GameObject explosionEffectPrefab;
    public float duration = 0.5f; // ���� ����Ʈ ���� �ð�

    private void Start()
    {
        Invoke(nameof(Explode), explodeDelay);
    }

    public void Explode()
    {
        int power = GameInstance.Instance.GetCharacterStat(ownerCharacterId).GetBombPower();
        Debug.Log($"[Bomb] {ownerCharacterId}�� ���� ����: {power}");

        // �߽� ����
        CreateExplosion(transform.position, Vector2.zero);

        // ���⺰ ����
        ExplodeInDirection(Vector2.up, power);
        ExplodeInDirection(Vector2.down, power);
        ExplodeInDirection(Vector2.left, power);
        ExplodeInDirection(Vector2.right, power);

        Destroy(gameObject);
    }

    private void ExplodeInDirection(Vector2 dir, int range)
    {
        Vector2 origin = transform.position;

        for (int i = 1; i <= range; i++)
        {
            Vector2 targetPos = origin + dir * i;

            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.45f);

            bool blocked = false;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Indestructible"))
                {
                    blocked = true;
                    break; // �������� ����Ʈ�� ������
                }

                if (hit.TryGetComponent<DestructibleBox>(out var box))
                {
                    box.DestroyBox();
                    blocked = true;
                    break;
                }
            }

            // �׻� ����Ʈ ���� ����
            CreateExplosion(targetPos, dir);

            // ������ ���⼭ ����� (�� ���� ĭ���� �� ��)
            if (blocked)
                return;
        }
    }

    private void CreateExplosion(Vector2 position, Vector2 direction)
    {
        if (explosionEffectPrefab == null) return;

        GameObject explosion = Instantiate(explosionEffectPrefab, position, Quaternion.identity);

        if (explosion.TryGetComponent<Explosion>(out var explosionScript))
        {
            explosionScript.SetOwner(ownerCharacterId);
            // ���⺰ �ð��� ó�� (��: �߾�, ��, ����)
            if (direction == Vector2.zero)
                explosionScript.SetActiveRenderer(explosionScript.middle); // �߽�
            else
                explosionScript.SetActiveRenderer(explosionScript.middle); // �ʿ�� end/side �б�

            explosionScript.SetDirection(direction);
            explosionScript.DestroyAfter(duration); // �ڵ� ����
        }
    }
}

