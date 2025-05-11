using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    private Collider2D swordCollider;

    public int damageAmount = 3;
    public float knockbackForce = 0.1f;

    public enum AttackDirection { right, left, up, down }
    public AttackDirection attackDirection;

    public Vector3 faceRight = new Vector3(0, 0, 0);
    public Vector3 faceLeft = new Vector3(-0.32f, 0, 0);
    public Vector3 faceUp = new Vector3(-0.14f, 0.20f, 0);
    public Vector3 faceDown = new Vector3(-0.14f, -0.11f, 0);

    private void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        swordCollider.enabled = false;
    }

    public void Attack()
    {
        swordCollider.enabled = true;

        switch (attackDirection)
        {
            case AttackDirection.right:
                transform.localPosition = faceRight;
                break;
            case AttackDirection.left:
                transform.localPosition = faceLeft;
                break;
            case AttackDirection.up:
                transform.localPosition = faceUp;
                break;
            case AttackDirection.down:
                transform.localPosition = faceDown;
                break;
        }

        Debug.Log($"Ataque para {attackDirection} ativado!");
    }

    public void StopAttack()
    {
        swordCollider.enabled = false;
    }

    public void SetAttackDirection(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            attackDirection = direction.x > 0 ? AttackDirection.right : AttackDirection.left;
        }
        else
        {
            attackDirection = direction.y > 0 ? AttackDirection.up : AttackDirection.down;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!swordCollider.enabled) return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Vector2 dir = GetDirectionVector();
            damageable.TakeDamage(damageAmount, dir, knockbackForce);


            // Knockback se tiver Rigidbody2D
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 forceDir = GetDirectionVector();
                rb.AddForce(forceDir * knockbackForce, ForceMode2D.Impulse);
                Debug.Log("Knockback aplicado!");
            }
        }
    }

    private Vector2 GetDirectionVector()
    {
        switch (attackDirection)
        {
            case AttackDirection.right: return Vector2.right;
            case AttackDirection.left: return Vector2.left;
            case AttackDirection.up: return Vector2.up;
            case AttackDirection.down: return Vector2.down;
            default: return Vector2.zero;
        }
    }
}
