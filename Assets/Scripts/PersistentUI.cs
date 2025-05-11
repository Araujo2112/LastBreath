using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Faz com que o GameObject n�o seja destru�do ao mudar de cena
        }
        else
        {
            Destroy(gameObject); // Evita duplica��o
        }
    }
}
