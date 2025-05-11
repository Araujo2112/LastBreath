using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonEntrada : MonoBehaviour
{
    public string targetScene = "Dungeon";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);
            Debug.Log("Carregando cena: " + targetScene);
        }
    }
}
