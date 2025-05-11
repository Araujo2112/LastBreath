using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea] public string tutorialMessage;
    public GameObject tutorialUI;
    public float fadeInSpeed = 2f;
    public float fadeOutSpeed = 2f;
    public float displayDuration = 3f;

    private CanvasGroup canvasGroup;
    private float timer;
    private bool fadingIn = false;
    private bool fadingOut = false;

    private void Start()
    {
        if (tutorialUI == null)
        {
            Debug.LogError("tutorialUI não está atribuído!");
            return;
        }

        canvasGroup = tutorialUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = tutorialUI.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMessage(tutorialMessage);
        }
    }

    private void ShowMessage(string message)
    {
        tutorialUI.GetComponentInChildren<Text>().text = message;
        fadingIn = true;
        fadingOut = false;
        timer = displayDuration;
        canvasGroup.alpha = 0; 
        tutorialUI.SetActive(true);
    }

    private void Update()
    {
        if (fadingIn)
        {
            canvasGroup.alpha += Time.deltaTime * fadeInSpeed;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
                fadingIn = false;
            }
        }
        else if (!fadingIn && !fadingOut && timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                fadingOut = true;
            }
        }
        else if (fadingOut)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeOutSpeed;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                fadingOut = false;
                tutorialUI.SetActive(false); 
            }
        }
    }
}
