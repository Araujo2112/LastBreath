using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    public CanvasGroup gameOverCanvas;
    public Text continueText;
    public float blinkSpeed = 0.5f;

    private bool isShowing = false;
    private float timer;
    private PlayerController player;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        HideGameOver();
    }

    void Update()
    {
        if (!isShowing) return;

        timer += Time.deltaTime;
        if (timer > 2f)
        {
            continueText.enabled = Mathf.FloorToInt((timer - 2f) / blinkSpeed) % 2 == 0;

            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                ContinueGame();
            }
        }
    }

    public void ShowGameOver(PlayerController deadPlayer)
    {
        Debug.Log("ShowGameOver chamado!");

        player = deadPlayer;
        isShowing = true;
        timer = 0f;

        player.animator.SetTrigger("death");

        gameOverCanvas.alpha = 1;
        gameOverCanvas.blocksRaycasts = true;
        gameOverCanvas.interactable = true;

        continueText.enabled = false;
    }

    private void ContinueGame()
    {
        isShowing = false;
        HideGameOver();

        player.currentHealth = player.maxHealth;
        PlayerPrefs.SetInt("PlayerHealth", player.maxHealth);
        PlayerPrefs.Save();

        if (player.healthBar != null)
        {
            player.healthBar.gameObject.SetActive(true);
            player.healthBar.ResetHealthBar(player.maxHealth);
        }

        player.UnlockMoviment();
        player.animator.ResetTrigger("death");
        player.animator.SetBool("isMoving", false);
        player.isInDialogue = false;
        player.rb.linearVelocity = Vector3.zero;

        foreach (Collider2D col in player.GetComponents<Collider2D>())
            col.enabled = true;

        player.GetComponent<Rigidbody2D>().isKinematic = false;
        player.GetComponent<SpriteRenderer>().color = Color.white;

        SaveData saveData = SaveManager.LoadGame();
        if (saveData != null)
        {
            player.transform.position = saveData.GetPosition();
            Debug.Log("Jogador reposicionado para checkpoint " + saveData.gameProgress);
        }
        else
        {
            player.transform.position = Vector3.zero;
        }

        if (player.sceneMusicController != null && player.sceneMusicController.sceneMusic != null)
        {
            AudioManager.Instance.musicaBloqueada = false;
            AudioManager.Instance?.PlayMusic(player.sceneMusicController.sceneMusic, true, 2f);
        }

        typeof(PlayerController).GetField("isInvincible", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(player, false);
    }

    private void HideGameOver()
    {
        gameOverCanvas.alpha = 0;
        gameOverCanvas.blocksRaycasts = false;
        gameOverCanvas.interactable = false;
    }
}
