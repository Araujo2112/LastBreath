using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour, IDamageable
{
    // ─────────────────────────────────────────────
    //              COMPONENTS & STATE
    // ─────────────────────────────────────────────
    [Header("Movement Settings")]
    [Tooltip("Velocidade de movimento do jogador")]
    public float moveSpeed = 1f;
    [Tooltip("Offset usado para detectar colisões")]
    public float collisionOffset = 0.05f;
    [Tooltip("Filtro de colisão usado para detectar obstáculos")]
    public ContactFilter2D movementFilter;
    [HideInInspector] public bool isInDialogue = false;

    [Header("Attack Settings")]
    [Tooltip("Componente que gerencia o ataque de espada")]
    public SwordAttack swordAttack;
    [Tooltip("Duração da animação de ataque")]
    public float attackDuration = 0.2f;
    [Tooltip("Dano causado por ataque")]
    public int attackDamage = 1;

    [Header("Health Settings")]
    [Tooltip("Barra de vida")]
    public HealthBar healthBar;
    [Tooltip("Vida máxima do jogador")]
    public int maxHealth = 5;
    [Tooltip("Vida atual do jogador")]
    public int currentHealth;
    [Tooltip("Força do knockback ao levar dano")]
    public float knockbackForce = 5f;
    [Tooltip("Duração do knockback")]
    public float knockbackDuration = 0.2f;

    [Header("Dash Settings")]
    [Tooltip("Cooldown entre dashes")]
    public float dashCooldown = 1f;
    private bool isDashing = false;
    private float lastDashTime;
    private TrailRenderer trailRenderer;
    public GameObject dashEffect;

    [Header("Death Settings")]
    [Tooltip("Som de morte")]
    public AudioClip deathMusic;
    [Tooltip("Controller das musicas das cenas")]
    public SceneMusicController sceneMusicController;
    [Tooltip("Delay do som")]
    public float deathMusicDelay = 1f;


    // Privados
    Vector2 movementInput;
    Vector2 lastMovementDirection = Vector2.down;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    bool canMove = true;
    bool isKnockedBack = false;
    float knockbackTimer = 0f;
    private bool isInvincible = false;
    private InteractionDetector interactionDetector;

    // ─────────────────────────────────────────────
    //                  UNITY METHODS
    // ─────────────────────────────────────────────
    void Start()
    {

        if (dashEffect != null)
            dashEffect.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        interactionDetector = GetComponentInChildren<InteractionDetector>();

        healthBar = FindObjectOfType<HealthBar>();

        SaveData saveData = SaveManager.LoadGame();
        if (saveData != null)
        {
            transform.position = saveData.GetPosition();
            currentHealth = saveData.health;
            maxHealth = saveData.maxHealth;
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }


    private void FixedUpdate()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        if (canMove)
        {
            HandleMovement();
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    // ─────────────────────────────────────────────
    //                 MOVEMENT
    // ─────────────────────────────────────────────
    public void LockMoviment() => canMove = false;
    public void UnlockMoviment() => canMove = true;

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    private void HandleMovement()
    {
        if (isInDialogue)
        {
            animator.SetBool("isMoving", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            if (!success) success = TryMove(new Vector2(movementInput.x, 0));
            if (!success) success = TryMove(new Vector2(0, movementInput.y));

            animator.SetBool("isMoving", success);

            if (success)
            {
                lastMovementDirection = movementInput;

                if (Mathf.Abs(movementInput.x) > Mathf.Abs(movementInput.y))
                {
                    animator.SetFloat("moveX", movementInput.x);
                    animator.SetFloat("moveY", 0);
                    spriteRenderer.flipX = movementInput.x < 0;
                }
                else
                {
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", movementInput.y);
                }
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            castCollisions.Clear();
            int count = rb.Cast(direction, movementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);

            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
        }

        return false;
    }

    // ─────────────────────────────────────────────
    //                 ATTACK
    // ─────────────────────────────────────────────
    void OnAttack(InputValue value)
    {
        if (isInDialogue) return;

        Debug.Log("Attack action triggered!");
        animator.SetTrigger("swordAttack");
        LockMoviment();

        if (Mathf.Abs(lastMovementDirection.x) > Mathf.Abs(lastMovementDirection.y))
            swordAttack.attackDirection = lastMovementDirection.x > 0 ? SwordAttack.AttackDirection.right : SwordAttack.AttackDirection.left;
        else
            swordAttack.attackDirection = lastMovementDirection.y > 0 ? SwordAttack.AttackDirection.up : SwordAttack.AttackDirection.down;

        swordAttack.Attack();
        Invoke("StopAttack", attackDuration);
    }

    void StopAttack()
    {
        swordAttack.StopAttack();
        UnlockMoviment();
    }

    // ─────────────────────────────────────────────
    //                 DASH
    // ─────────────────────────────────────────────
    private IEnumerator Dash()
    {
        isDashing = true;
        isInvincible = true;
        canMove = false;

        if (trailRenderer != null)
            trailRenderer.emitting = true;

        Vector2 dashDir = movementInput != Vector2.zero ? movementInput.normalized : lastMovementDirection;

        if (dashEffect != null)
        {
            Vector2 offset = -dashDir * 0.3f;
            dashEffect.transform.position = (Vector2)transform.position + offset;

            float angle = Mathf.Atan2(-dashDir.y, -dashDir.x) * Mathf.Rad2Deg + -90f;
            dashEffect.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            dashEffect.SetActive(true);
        }

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        float dashDistance = 0.5f;
        float dashSpeed = 5f;
        float distanceMoved = 0f;

        while (distanceMoved < dashDistance)
        {
            float step = dashSpeed * Time.fixedDeltaTime;
            Vector2 nextPosition = rb.position + dashDir * step;

            RaycastHit2D hit = Physics2D.Raycast(rb.position, dashDir, step, LayerMask.GetMask("Default"));
            if (hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                break;
            }

            rb.MovePosition(nextPosition);
            distanceMoved += step;
            yield return new WaitForFixedUpdate();
        }

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);

        if (trailRenderer != null)
            trailRenderer.emitting = false;

        if (dashEffect != null)
            StartCoroutine(DisableDashEffectAfterAnim());

        canMove = true;
        isDashing = false;
        isInvincible = false;
        lastDashTime = Time.time;
    }


    void OnDash(InputValue value)
   {
       Debug.Log("DASH PRESSIONADO");
       if (!isDashing && Time.time >= lastDashTime + dashCooldown)
       {
           StartCoroutine(Dash());
       }
   }

    private IEnumerator DisableDashEffectAfterAnim()
    {
        Animator anim = dashEffect.GetComponent<Animator>();
        if (anim != null)
        {
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(animLength);
        }

        dashEffect.SetActive(false);
    }


    // ─────────────────────────────────────────────
    //              INTERACTION
    // ─────────────────────────────────────────────
    void OnInteract(InputValue value)
    {
        interactionDetector?.ManualInteract();
    }


    // ─────────────────────────────────────────────
    //               DAMAGE / HEALTH
    // ─────────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, Vector2.zero, 0f);
    }

    public void TakeDamage(int amount, Vector2 knockbackDir, float force)
    {
        if (isInvincible) return;

        currentHealth -= amount;

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        PlayerPrefs.SetInt("PlayerHealth", currentHealth);
        PlayerPrefs.Save();

        StartCoroutine(FlashRed());

        if (force > 0f)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(knockbackDir.normalized * force, ForceMode2D.Impulse);
            isKnockedBack = true;
            knockbackTimer = knockbackDuration;
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DieRoutine());
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private IEnumerator DieRoutine()
    {
        animator.SetTrigger("death");
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        isInvincible = true;

        foreach (Collider2D col in GetComponents<Collider2D>())
            col.enabled = false;

        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        currentHealth = maxHealth;
        PlayerPrefs.SetInt("PlayerHealth", maxHealth);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.musicaBloqueada = true;
            AudioManager.Instance.StopMusic(1f);
        }

        yield return new WaitForSeconds(deathMusicDelay);

        if (deathMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(deathMusic);
        }

        yield return new WaitForSeconds(2f);

        ArenaController arenaController = FindObjectOfType<ArenaController>();
        if (arenaController != null)
        {
            arenaController.ResetArena();
        }

        ArenaTimer arenaTimer = FindObjectOfType<ArenaTimer>();
        if (arenaTimer != null)
        {
            arenaTimer.ResetArena();
        }


        if (GameOverManager.Instance != null)
        {
            GameOverManager.Instance.ShowGameOver(this);
        }
    }


    // ─────────────────────────────────────────────
    //                     SAVE
    // ─────────────────────────────────────────────
    public void SavePlayer(int gameProgress, bool arenaCompleta)
    {
        SaveManager.SaveGame(this, gameProgress, SaveManager.IsArenaTimerComplete(), SaveManager.IsArenaWaveComplete());
    }
}
