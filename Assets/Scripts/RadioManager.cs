using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Analytics; 
using static EventManager;

public class RadioManager : MonoBehaviour
{
    public AudioSource ruidosDisturbios;
    public AudioSource audioSeguridad;
    public GameObject panelDialogo;
    public TextMeshProUGUI textoDialogo;
    public List<string> mensajesDisturbios = new List<string>
{
    "Un infectado generó disturbios, más cuidado con quien dejas entrar.",
    "Se reportan más incidentes dentro del búnker, la situación está empeorando.",
    "Otro infectado ha causado problemas, la seguridad está en alerta.",
    "El número de infectados causando disturbios está aumentando rápidamente.",
    "Se detectó otro brote de violencia dentro del búnker, ¡Estamos en estado de emergencia!"
};
    public float velocidadEscritura = 0.05f;
    public float duracionMensaje = 5f;

    private bool estaEscribiendo = false;

    public Slider barraContaminacion;
    public float maxContaminacion = 3f;  
    private float nivelContaminacion = 0f;  
    public Image fillBarImage;

    public Color colorAmarillo = Color.yellow;
    public Color colorNaranja = new Color(1f, 0.5f, 0f);
    public Color colorRojo = Color.red;

    public RectTransform panelPerdiste;

    public Animator radioAnimator;

    private int indiceMensajeActual = 0;

    public s_GameManager gameManager;
    public CharactersManager charactersManager;

     public AudioSource loadingBar;
      public AudioSource sonidoEstatica;


    void Start()
    {
        if (barraContaminacion != null)
        {
            barraContaminacion.maxValue = maxContaminacion;
            barraContaminacion.value = nivelContaminacion;
        }

    }

    public void ActivarDisturbios()
    {
        StartCoroutine(ActivarDisturbiosCoroutine());
    }

    public IEnumerator ActivarDisturbiosCoroutine()
    {

        yield return new WaitForSeconds(10f);

        ActualizarContaminacion(1);

        if (radioAnimator != null)
        {
            radioAnimator.SetTrigger("ActivarRadio");
            Debug.Log("Animación ActivarRadio desencadenada");
        }
        else
        {
            Debug.LogWarning("radioAnimator es nulo");
        }
        ruidosDisturbios.Play();
        audioSeguridad.Play();

        panelDialogo.SetActive(true);  
        StartCoroutine(EscribirTexto(mensajesDisturbios[indiceMensajeActual]));


        while (estaEscribiendo)
        {
            yield return null;
        }

        yield return new WaitForSeconds(duracionMensaje);

        panelDialogo.SetActive(false);

        audioSeguridad.Stop();

        yield return new WaitForSeconds(4f);

        ruidosDisturbios.Stop();


        if (radioAnimator != null)
        {
            radioAnimator.SetTrigger("IdleRadio"); 
        }

        indiceMensajeActual = (indiceMensajeActual + 1) % mensajesDisturbios.Count;


        yield return new WaitForSeconds(2f);

        gameManager.NextCharacter();

    }


    private IEnumerator EscribirTexto(string mensaje)
    {
        estaEscribiendo = true;
        textoDialogo.text = "";

        foreach (char letra in mensaje.ToCharArray())
        {
            textoDialogo.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }

        estaEscribiendo = false;
    }


    public void ActualizarContaminacion(float cantidad)
    {
        nivelContaminacion += cantidad;

        // Asegurarse de no exceder el nivel máximo de contaminación
        if (nivelContaminacion > maxContaminacion)
        {
            nivelContaminacion = maxContaminacion;
        }

       
        if (barraContaminacion != null)
        {
            barraContaminacion.value = nivelContaminacion;
            ActualizarColorBarra();
            loadingBar.Play();
        }

        if (nivelContaminacion >= maxContaminacion)
        {

            Debug.Log("¡El búnker está completamente contaminado! Has perdido.");
            
             Perder();
                   
        }
    }


    private void ActualizarColorBarra()
    {
        float porcentajeContaminacion = nivelContaminacion / maxContaminacion;

        if (porcentajeContaminacion <= 0.34f)
        {
            fillBarImage.color = colorAmarillo; 
        }
        else if (porcentajeContaminacion <= 0.67f)
        {
            fillBarImage.color = colorNaranja; 
        }
        else
        {
            fillBarImage.color = colorRojo; 
        }
    }


    private void Perder()
    {
    
       RegisterGameOverEvent();

        CharactersManager charactersManager = FindObjectOfType<CharactersManager>();
        if (charactersManager != null)
        {
            charactersManager.DetenerPersonajes();  
        }


        panelPerdiste.gameObject.SetActive(true);
    }

       private void RegisterGameOverEvent()
{
     // Debug para verificars
    Debug.Log($"[DEBUG] GameOver registrado - Nivel: {GameData.NivelActual}, Razón: Fired");

    // Crear y configurar el evento
    GameOverEvent gameOver = new GameOverEvent();
    gameOver.level = GameData.NivelActual;
    gameOver.reason = "Fired";

    // Grabar el evento 
    #if !UNITY_EDITOR
        AnalyticsService.Instance.RecordEvent(gameOver);
    #else
        Debug.Log("[ANALYTICS] Evento GameOverEvent registrado");
    #endif
}

    public void PantallaRadio()
    {

        StartCoroutine(ActivarPantallaRadio());

    }

    public IEnumerator ActivarPantallaRadio()
    {
        radioAnimator.SetTrigger("ActivarRadio");
        sonidoEstatica.Play();
        yield return new WaitForSeconds(2f);
        sonidoEstatica.Stop();
        radioAnimator.SetTrigger("IdleRadio");
    }


}
