using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

    public class MainMenu : MonoBehaviour
    {
        public GameObject continueButton;
        public GameObject newGameButton;
        public GameObject settingsButton;
        public GameObject exitButton;

        public GameObject mainMenuPanel;
        public GameObject settingsPanel;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);

        if (SaveManager.SaveExists())
            continueButton.SetActive(true);
        else
            continueButton.SetActive(false);

        newGameButton.SetActive(true);
        settingsButton.SetActive(true);
        exitButton.SetActive(true);
    }

    public void NewGame()
    {
        SaveManager.DeleteSave();
        SceneManager.LoadScene("Game1");
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Game1");
    }


    public void Settings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
