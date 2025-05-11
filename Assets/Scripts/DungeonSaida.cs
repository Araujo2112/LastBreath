using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonSaida : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("Game1");
        }
    }
}

// falta meter a sair para a saida da dungeon e n�o para o Game1