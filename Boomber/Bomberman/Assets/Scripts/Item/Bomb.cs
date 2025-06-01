using UnityEngine;

public class Bomb : MonoBehaviour
{
    public string ownerCharacterId;
    public float explodeDelay = 2f;
    public GameObject explosionEffectPrefab;
    public float duration = 0.5f; // 폭발 이펙트 지속 시간

    private void Start()
    {
        Invoke(nameof(Explode), explodeDelay);
    }

    public void Explode()
    {
        int power = GameInstance.Instance.GetCharacterStat(ownerCharacterId).GetBombPower();
        Debug.Log($"[Bomb] {ownerCharacterId}의 폭발 범위: {power}");

        // 중심 폭발
        CreateExplosion(transform.position, Vector2.zero);

        // 방향별 폭발
        ExplodeInDirection(Vector2.up, power);
        ExplodeInDirection(Vector2.down, power);
        ExplodeInDirection(Vector2.left, power);
        ExplodeInDirection(Vector2.right, power);

        Destroy(gameObject);
    }

    private void ExplodeInDirection(Vector2 dir, int range)
    {
        Vector2 origin = transform.position;

        for (int i = 1; i <= range; i++) // i = 1부터 (중심 제외)
        {
            Vector2 targetPos = origin + dir * i;

            Collider2D[] hits = Physics2D.OverlapCircleAll(targetPos, 0.1f);

            // 먼저 벽 판별
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Indestructible"))
                {
                    return; // 폭발 차단
                }

                if (hit.TryGetComponent<DestructibleBox>(out var box))
                {
                    box.DestroyBox(); // 상자 파괴
                    CreateExplosion(targetPos, dir);
                    return; // 여기서도 멈춤
                }
            }

            // 아무 것도 없으면 폭발 진행
            CreateExplosion(targetPos, dir);
        }
    }

    private void CreateExplosion(Vector2 position, Vector2 direction)
    {
        if (explosionEffectPrefab == null) return;

        GameObject explosion = Instantiate(explosionEffectPrefab, position, Quaternion.identity);

        if (explosion.TryGetComponent<Explosion>(out var explosionScript))
        {
            // 방향별 시각적 처리 (예: 중앙, 끝, 방향)
            if (direction == Vector2.zero)
                explosionScript.SetActiveRenderer(explosionScript.middle); // 중심
            else
                explosionScript.SetActiveRenderer(explosionScript.middle); // 필요시 end/side 분기

            explosionScript.SetDirection(direction);
            explosionScript.DestroyAfter(duration); // 자동 제거
        }
    }
}

