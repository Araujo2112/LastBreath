using UnityEngine;
using UnityEngine.UI;

public class CheckpointMessage : MonoBehaviour
{
    public static CheckpointMessage Instance;
    public Text messageText;
    public float displayDuration = 2f;
    public float fadeSpeed = 2f;
    public float fadeInSpeed = 2f;

    private CanvasGroup canvasGroup;
    private float timer;
    private bool fadingIn = false;
    private bool fadingOut = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        fadingIn = true;
        fadingOut = false;
        canvasGroup.alpha = 0;
        timer = displayDuration;
    }

    void Update()
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
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                fadingOut = false;
            }
        }
    }
}
