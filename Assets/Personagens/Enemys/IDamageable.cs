using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int amount);
    void TakeDamage(int amount, Vector2 knockbackDirection, float force);
}

