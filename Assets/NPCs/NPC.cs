using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue firstDialogueData;
    public NPCDialogue secondDialogueData;
    private GameObject dialoguePanel;
    private Text dialogueText, nameText;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;
    private PlayerController playerController;
    private bool hasTalkedBefore = false;

    [Header("Final Decision Settings")]
    public bool isFinalGuardiao = false;
    private GameObject choicePanel;
    private Button yesButton;
    private Button noButton;

    [Header("Final Story UI")]
    public GameObject finalStoryCanvas;
    public Text finalStoryText;
    public string goodFinal;
    public string badFinal;

    void Start()
    {
        playerController = Object.FindFirstObjectByType<PlayerController>();

        dialoguePanel = DialogueUIController.Instance.dialoguePanel;
        dialogueText = DialogueUIController.Instance.dialogueText;
        nameText = DialogueUIController.Instance.nameText;

        choicePanel = DialogueUIController.Instance.choicePanel;
        yesButton = DialogueUIController.Instance.yesButton;
        noButton = DialogueUIController.Instance.noButton;

        // Procura segura dentro do DontDestroyOnLoad
        if (finalStoryCanvas == null)
        {
            foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.name == "FinalStory")
                {
                    finalStoryCanvas = go;
                    break;
                }
            }
        }

        if (finalStoryText == null)
        {
            foreach (Text t in Resources.FindObjectsOfTypeAll<Text>())
            {
                if (t.name == "FinalStoryText")
                {
                    finalStoryText = t;
                    break;
                }
            }
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        if (finalStoryCanvas != null)
            finalStoryCanvas.SetActive(false);
    }


    void Update()
    {
        if (isDialogueActive && !isTyping && Input.GetKeyDown(KeyCode.E))
        {
            NextLine(currentDialogueData);
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    public void Interact()
    {
        if (isDialogueActive) return;

        if (hasTalkedBefore)
            StartDialogue(secondDialogueData);
        else
            StartDialogue(firstDialogueData);
    }

    NPCDialogue currentDialogueData;

    void StartDialogue(NPCDialogue dialogueData)
    {
        currentDialogueData = dialogueData;
        isDialogueActive = true;
        dialogueIndex = 0;
        playerController.isInDialogue = true;

        FacePlayer();

        nameText.text = dialogueData.NPCName;
        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine(dialogueData));
    }

    void NextLine(NPCDialogue dialogueData)
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = (dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }
        else if (++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine(dialogueData));
        }
        else
        {
            EndDialogue();
            if (!hasTalkedBefore)
            {
                hasTalkedBefore = true;
                GiveItem();
            }
        }
    }

    IEnumerator TypeLine(NPCDialogue dialogueData)
    {
        isTyping = true;
        dialogueText.text = "";

        if (dialogueData.voiceSound != null)
        {
            AudioManager.Instance?.PlayDialogueSound(dialogueData.voiceSound, dialogueData.voicePitch);
        }

        foreach (char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        AudioManager.Instance?.StopDialogueSound();

        if (dialogueData.autoProgressLines.Length > dialogueIndex &&
            dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine(dialogueData);
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        AudioManager.Instance?.StopDialogueSound();
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        if (isFinalGuardiao && choicePanel != null)
        {
            choicePanel.SetActive(true);
            yesButton.onClick.RemoveAllListeners();
            noButton.onClick.RemoveAllListeners();

            yesButton.onClick.AddListener(() => HandleFinalChoice(true));
            noButton.onClick.AddListener(() => HandleFinalChoice(false));
        }
        else
        {
            playerController.isInDialogue = false;
        }
    }

    void FacePlayer()
    {
        playerController.animator.SetBool("isMoving", true);
        Vector2 dir = (transform.position - playerController.transform.position).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            playerController.animator.SetFloat("moveX", 1);
            playerController.animator.SetFloat("moveY", 0);
            playerController.GetComponent<SpriteRenderer>().flipX = dir.x < 0;
        }
        else
        {
            playerController.animator.SetFloat("moveX", 0);
            playerController.animator.SetFloat("moveY", dir.y > 0 ? 1 : -1);
        }
    }

    void GiveItem()
    {
        Debug.Log("Item given to the player.");
    }

    // ─────────────────────────────────────────────
    //                 FINAL JOGO
    // ─────────────────────────────────────────────

    void HandleFinalChoice(bool choseYes)
    {
        choicePanel.SetActive(false);
        playerController.isInDialogue = false;

        if (finalStoryCanvas == null || finalStoryText == null)
        {
            Debug.LogError("Final story canvas or text not assigned or was destroyed.");
            return;
        }

        finalStoryCanvas.SetActive(true);
        string finalText = choseYes ? goodFinal : badFinal;
        StartCoroutine(WriteFinalText(finalText));
    }


    IEnumerator WriteFinalText(string texto)
    {
        finalStoryText.text = "";

        texto = texto.Replace("\\n", "\n");

        foreach (char letra in texto)
        {
            finalStoryText.text += letra;
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(15f);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        SaveManager.DeleteSave();

        Application.Quit(); 
    }
}
