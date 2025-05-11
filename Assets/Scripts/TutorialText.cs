using UnityEngine;
using UnityEngine.UI;

public class TutorialText : MonoBehaviour
{
    public Text messageText;

    private void Awake()
    {
        if (messageText == null)
        {
            messageText = GetComponentInChildren<Text>();
        }
    }

    public void ShowMessage(string msg)
    {
        messageText.text = msg;
        gameObject.SetActive(true);
        Invoke("Hide", 3f);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
