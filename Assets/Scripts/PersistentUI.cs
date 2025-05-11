using UnityEngine;

public class PersistentUI : MonoBehaviour
{
    private static PersistentUI instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Faz com que o GameObject não seja destruído ao mudar de cena
        }
        else
        {
            Destroy(gameObject); // Evita duplicação
        }
    }
}
