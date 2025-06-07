using UnityEngine;

public class Explosion : MonoBehaviour
{
    public AnimatedSpriteRenderer start;
    public AnimatedSpriteRenderer middle;
    public AnimatedSpriteRenderer end;
    private string ownerId;
    public void SetActiveRenderer(AnimatedSpriteRenderer renderer)
    {
        start.enabled = renderer == start;
        middle.enabled = renderer == middle;
        end.enabled = renderer == end;
    }

    public void SetDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void DestroyAfter(float seconds)
    {
        Destroy(gameObject, seconds);
    }

    public void SetOwner(string id)
    {
        ownerId = id;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        AICharacter character = other.GetComponent<AICharacter>();
        if (character == null) return;

        if (character.characterId == ownerId)
        {
            Debug.Log($"[폭발] 자기 자신은 피해 없음: {ownerId}");
            return;
        }

        character.TakeDamage(1);
        Debug.Log($"[폭발] {character.characterId}이 폭발에 맞음");
    }

}
