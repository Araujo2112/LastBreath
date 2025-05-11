using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeathSlime : MonoBehaviour, IDamageable
{
    [Header("Estatísticas")]
    public int maxHealth = 5;
    public int Health { get; set; }

    [Header("Movimento")]
    public float walkSpeed = 0.3f;
    public float chaseSpeed = 0.5f;
    public float chaseDistance = 5f;
    public float attackDistance = 0.5f;
    public float knockbackForce = 0.3f;
    public float knockbackDuration = 0.2f;
    public float patrolPauseDuration = 2f;
    public List<Transform> patrolPoints;

    [Header("Ataque")]
    public int attackDamage = 1;
    public float attackCooldown = 3f;

    [Header("Áudio")]
    public AudioClip moveSound;
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip hurtSound;
    public float soundVolume = 0.8f;

    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private float lastAttackTime;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;

    private void Start()
    {
        Health = maxHealth;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (patrolPoints.Count > 0)
        {
            transform.position = patrolPoints[0].position;
            StartCoroutine(Patrol());
        }
    }

    private void Update()
    {
        if (player == null || Health <= 0) return;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Movimento e Ataque separados
        if (distanceToPlayer <= attackDistance)
        {
            // Parar o movimento quando no alcance de ataque
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);

            // Verifica se pode atacar
            TryAttack();
        }

        
        else if (distanceToPlayer <= chaseDistance)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * chaseSpeed;

            // Definir o movimento principal
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Movimento horizontal (left ou right)
                animator.SetFloat("moveX", direction.x);
                animator.SetFloat("moveY", 0);

                // Inverte o sprite para left e right
                spriteRenderer.flipX = direction.x < -0.1f;
            }
            else
            {
                // Movimento vertical (up ou down)
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", direction.y);

                // Certifica que o sprite não vira para cima ou baixo
                spriteRenderer.flipX = false;
            }

            animator.SetBool("isRunning", true);
            animator.SetBool("isMoving", false);

            PlaySound(moveSound);
            isPatrolling = false;
        }



        else if (!isPatrolling)
        {
            // Retomar patrulha se o jogador sair do alcance de perseguição
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            animator.SetBool("isMoving", false);
            StartCoroutine(Patrol());
        }
    }

private void TryAttack()
{
    if (Time.time - lastAttackTime >= attackCooldown)
    {
        // Calcula a direção do ataque
        Vector2 attackDirection = (player.position - transform.position).normalized;
        
        // Atualiza o flipX **antes** de iniciar o ataque
        if (Mathf.Abs(attackDirection.x) > 0.1f)
        {
            spriteRenderer.flipX = attackDirection.x < 0;
        }

        // Inicia a animação de ataque
        animator.SetTrigger("attack");
        lastAttackTime = Time.time;
        PlaySound(attackSound);
    }
}


    public void ApplyDamage()
    {
        if (Vector2.Distance(transform.position, player.position) <= attackDistance + 0.2f)
        {
            IDamageable dmg = player.GetComponent<IDamageable>();
            Vector2 knockbackDir = (player.position - transform.position).normalized;
            dmg?.TakeDamage(attackDamage, knockbackDir, knockbackForce);
        }
    }



    public void TakeDamage(int amount, Vector2 knockbackDir, float force)
    {
        Health -= amount;
        Debug.Log($"Slime levou {amount} de dano! Vida restante: {Health}");

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir.normalized * force, ForceMode2D.Impulse);

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;

        animator.SetTrigger("hurt");
        StartCoroutine(FlashRed());

        PlaySound(hurtSound);

        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Slime morreu!");
        animator.SetTrigger("death");

        rb.mass = 100f;
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;

        PlaySound(deathSound);
        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    private IEnumerator Patrol()
    {
        isPatrolling = true;

        while (isPatrolling)
        {
            Transform targetPoint = patrolPoints[currentPatrolIndex];
            Vector2 direction = (targetPoint.position - transform.position).normalized;
            rb.linearVelocity = direction * walkSpeed;

            if (direction.x != 0)
                spriteRenderer.flipX = direction.x < 0;

            animator.SetBool("isMoving", true);

            while (Vector2.Distance(transform.position, targetPoint.position) > 0.1f)
            {
                yield return null;
            }

            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            yield return new WaitForSeconds(patrolPauseDuration);

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    public void TakeDamage(int amount)
    {
        // não usado, mas requerido pela interface
        throw new System.NotImplementedException();
    }
}
