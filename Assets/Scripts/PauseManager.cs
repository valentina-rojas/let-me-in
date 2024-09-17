using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
   public GameObject pauseMenuUI;  // Referencia al panel del menú de pausa

    private bool isPaused = false;

      private AudioSource[] allAudioSources;

       void Start()
    {
        // Encuentra todos los componentes AudioSource en la escena
        allAudioSources = FindObjectsOfType<AudioSource>();
    }

    void Update()
    {
        // Detecta si el jugador presiona la tecla de pausa (Escape por defecto)
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
        pauseMenuUI.SetActive(false);  // Oculta el menú de pausa
        Time.timeScale = 1f;           // Restablece el tiempo normal

        // Reanuda todos los sonidos
        foreach (AudioSource audio in allAudioSources)
        {
            audio.UnPause();
        }

        isPaused = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);   // Muestra el menú de pausa
        Time.timeScale = 0f;           // Detiene el tiempo en el juego

        // Pausa todos los sonidos
        foreach (AudioSource audio in allAudioSources)
        {
            audio.Pause();
        }

        isPaused = true;
    }

   /* public void RestartGame()
    {
        Time.timeScale = 1f;           // Restablece el tiempo normal
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Reinicia la escena actual
        pauseMenuUI.SetActive(false);  // Asegúrate de que el menú de pausa esté desactivado
    }*/
   public void ReturnToMenu()
    {
        Time.timeScale = 1f;           // Restablece el tiempo normal
        SceneManager.LoadScene("MenuPrincipal");  // Carga la escena del menú principal
        pauseMenuUI.SetActive(false);  // Asegúrate de que el menú de pausa esté desactivado
    }
}


