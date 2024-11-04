using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject PauseMenu;
    public GameObject GameMenu;
    public GameObject SettingsMenu;


    public void Start()
    {
        MainMenu.SetActive(true);
        PauseMenu.SetActive(false);
        SettingsMenu.SetActive(false);
        GameMenu.SetActive(false);
    }

    public void StartGame()
    {
        Debug.Log("Game start");
        ShowGameMenu();
    }
    
    public void OpenSettings()
    {
        Debug.Log("Settings menu opened");
        ShowSettingsMenu(true);
    }

    public void ConfirmSettings()
    {
        Debug.Log("Settings confirmed");
        ShowSettingsMenu(false);
    }

    public void GamePause(bool isPaused)
    {
        if (isPaused)
        {
            ShowPauseMenu();
        }
        else
        {
            ShowGameMenu();
        }
    }

    public void BackToMenu()
    {
        ShowMainMenu();
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }

    public void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        PauseMenu.SetActive(false);
        GameMenu.SetActive(false);
    }

    public void ShowSettingsMenu(bool isShowed)
    {
        SettingsMenu.SetActive(isShowed);
    }

    public void ShowGameMenu()
    {
        MainMenu.SetActive(false);
        PauseMenu.SetActive(false);
        GameMenu.SetActive(true);
    }

    public void ShowPauseMenu()
    {
        MainMenu.SetActive(false);
        PauseMenu.SetActive(true);
        GameMenu.SetActive(false);
    }
}