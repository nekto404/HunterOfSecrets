using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject PauseMenu;
    public GameObject GameMenu;
    public GameObject SettingsMenu;
    public GameObject LoseScreen;
    public GameObject WinScreen;
    public ConfirmationUI ConfirmationUI;

    public Slider MusicVolumeSlider;
    public Slider SFXVolumeSlider;

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
        GameManager.Instance.StartGame();
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
        SFXVolumeSlider.value = AudioController.Instance.GetSFXVolume();
        MusicVolumeSlider.value = AudioController.Instance.GetMusicVolume();
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

    public void SFXVolumeChanged()
    {
        AudioController.Instance.SetSFXVolume(SFXVolumeSlider.value);
    }

    public void MusicVolumeChanged()
    {
        AudioController.Instance.SetMusicVolume(MusicVolumeSlider.value);
    }

    public void ShowLoseScreen()
    {
    }

    public void ShowWinScreen()
    {
    }
}