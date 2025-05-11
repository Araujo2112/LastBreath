using UnityEngine;

public class GreenSlime : MonoBehaviour
{
    public int maxHealth = 5;
    public int Health { get; set; }

    Animator animator;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        Health = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        Debug.Log($"Slime levou {amount} de dano! Vida restante: {Health}");

        StartCoroutine(FlashRed());

        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Slime morreu!");
        // Aqui podes pôr animação de morte, som, partículas, etc.
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }
}
