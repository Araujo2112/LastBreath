using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public PlayerController playerController;
    public PlayerInput playerInput;
    private bool isPaused = false;

    void Start()
    {
        // Garante que o menu começa fechado
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Verifica se os objetos estão atribuídos
        if (playerController == null || playerInput == null)
        {
            Debug.LogError("PlayerController ou PlayerInput não estão atribuídos no Inspector!");
        }
    }

    void Update()
    {
        // Verifica se a tecla ESC foi pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        AudioListener.pause = true; // Pausa todo o áudio

        // Desativa os inputs do jogador
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }

        Debug.Log("Jogo Pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        AudioListener.pause = false; // Retoma todo o áudio

        // Ativa os inputs do jogador
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

        Debug.Log("Jogo Continuado");
    }

    public void OpenOptions()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
        Debug.Log("Menu de Opções Aberto");
    }

    public void BackToPauseMenu()
    {
        pauseMenuUI.SetActive(true);
        optionsMenuUI.SetActive(false);
        Debug.Log("Menu de Pausa Aberto");
    }

    public void BackToMainMenu()
    {
        // Garante que o tempo volta ao normal
        Time.timeScale = 1f;
        isPaused = false;
        AudioListener.pause = false; // Retoma o áudio

        // Desativa os menus para evitar que fiquem visíveis na próxima cena
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsMenuUI != null)
            optionsMenuUI.SetActive(false);

        // Destroi todos os objetos que não devem persistir ao mudar para o menu principal
        foreach (var obj in FindObjectsOfType<HealthBar>())
        {
            Destroy(obj.gameObject);
        }

        // Carrega o menu principal
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Voltando ao Menu Principal");
    }



    public void QuitGame()
    {
        Debug.Log("A Sair do Jogo...");
        Application.Quit();
    }
}
