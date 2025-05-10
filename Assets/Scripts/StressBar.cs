using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Analytics; 
using static EventManager;

public class StressBar : MonoBehaviour
{
    public Slider barraEstres;
    public float maxEstres = 5f;
    private float nivelEstres = 0f;
    public Image fillBarImage;
    public GameObject panelPerdiste;

    [Header("Colores de la barra de estrés")]
    public Color colorVerdeClaro;
    public Color colorVerdeOscuro;
    public Color colorAmarillo;
    public Color colorNaranja;
    public Color colorRojo;


    public CharactersManager charactersManager;

    public Animator mate;
    public AudioSource audioMate;
    public AudioSource loadingBar;

    void Start()
    {

        if (barraEstres != null)
        {
            barraEstres.maxValue = maxEstres;
            barraEstres.value = nivelEstres;
        }
    }


    public void ActualizarEstres(float cantidad)
    {
        nivelEstres += cantidad;


        if (nivelEstres > maxEstres)
        {
            nivelEstres = maxEstres;
        }

        if (barraEstres != null)
        {
            barraEstres.value = nivelEstres;
            ActualizarColorBarra();
            loadingBar.Play();
        }


        if (nivelEstres >= maxEstres)
        {
            PerderJuego();
        }
    }



    public void DisminuirEstres(float cantidad)
    {
        nivelEstres -= cantidad;

        StartCoroutine(AnimacionMate());

        if (nivelEstres < 0)
        {
            nivelEstres = 0;
        }


        if (barraEstres != null)
        {
            barraEstres.value = nivelEstres;
            ActualizarColorBarra();
        }
    }


    public IEnumerator AnimacionMate()
    {
        mate.SetTrigger("mateSpin");
        audioMate.Play();
        yield return new WaitForSeconds(2f);
        audioMate.Stop();
        mate.SetTrigger("mateIdle");
    }


    private void ActualizarColorBarra()
    {
        float porcentajeEstres = nivelEstres / maxEstres;

        if (porcentajeEstres <= 0.2f)
        {
            fillBarImage.color = colorVerdeClaro;  
        }
        else if (porcentajeEstres <= 0.4f)
        {
            fillBarImage.color = colorVerdeOscuro; 
        }
        else if (porcentajeEstres <= 0.6f)
        {
            fillBarImage.color = colorAmarillo;    
        }
        else if (porcentajeEstres <= 0.8f)
        {
            fillBarImage.color = colorNaranja;    
        }
        else
        {
            fillBarImage.color = colorRojo;     
        }
    }



    public void PerderJuego()
    {

        RegisterGameOverEvent();

        CharactersManager charactersManager = FindObjectOfType<CharactersManager>();
        if (charactersManager != null)
        {
            charactersManager.DetenerPersonajes();  
        }

        panelPerdiste.SetActive(true);
    }


   private void RegisterGameOverEvent()
{
     // Debug para verificars
    Debug.Log($"[DEBUG] GameOver registrado - Nivel: {GameData.NivelActual}, Razón: Quit");

    // Crear y configurar el evento
    GameOverEvent gameOver = new GameOverEvent();
    gameOver.level = GameData.NivelActual;
    gameOver.reason = "Quit";

    // Grabar el evento 
    #if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(gameOver);
    #else
        Debug.Log("[ANALYTICS] Evento GameOverEvent registrado");
    #endif
}
}
