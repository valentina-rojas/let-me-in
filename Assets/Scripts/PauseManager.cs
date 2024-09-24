using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;

    private bool isPaused = false;

    private AudioSource[] allAudioSources;

    void Start()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;

        // Reanuda todos los sonidos
        foreach (AudioSource audio in allAudioSources)
        {
            audio.UnPause();
        }

        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;

        // Pausa todos los sonidos
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Pause();
        }

        isPaused = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        pauseMenuUI.SetActive(false);
    }



    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Restablece el tiempo al valor normal
        GameData.NivelActual = 1; // Reinicia el nivel actual a 1
        SceneManager.LoadScene("MenuPrincipal"); // Carga la escena del menú principal
        pauseMenuUI.SetActive(false); // Desactiva el menú de pausa
    }

}


