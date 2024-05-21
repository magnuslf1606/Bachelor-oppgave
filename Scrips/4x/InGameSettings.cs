using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameSettings : MonoBehaviour
{
    public Button exit, closeGame, fullscreen, windowed;
    public GameObject canvas;
    void Start() {
        exit.onClick.AddListener(Exit);
        closeGame.onClick.AddListener(CloseGame);
        fullscreen.onClick.AddListener(SetFullScreen);
        windowed.onClick.AddListener(SetWindowed);
    }

    void Exit() {
        canvas.SetActive(false);
    }
    void CloseGame() {
        Application.Quit();
    }
    void SetFullScreen() {
        Screen.SetResolution(PlayerPrefs.GetInt("resX"), PlayerPrefs.GetInt("resY"), true);
    }
    void SetWindowed() {
        Screen.SetResolution(PlayerPrefs.GetInt("resX"), PlayerPrefs.GetInt("resY"), false);
    }
}
