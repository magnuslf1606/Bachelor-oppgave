using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    private bool isGamePaused = false;
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        // Sørg for at pausemenyen er deaktivert ved oppstart
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Sjekk om Escape-knappen er trykket
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        // Fryse spillet
        Time.timeScale = 0f;
        isGamePaused = true;

        // Aktivere pausemenyen
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
    }

    void ResumeGame()
    {
        // Gjenoppta spillet
        Time.timeScale = 1f;
        isGamePaused = false;

        // Deaktivere pausemenyen
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }
}

