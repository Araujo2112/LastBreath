using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public static DialogueUIController Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Text nameText;

    public GameObject choicePanel;
    public Button yesButton;
    public Button noButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
