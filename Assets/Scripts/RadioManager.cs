using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public float maxContaminacion = 3f;  // Máximo nivel de contaminación
    private float nivelContaminacion = 0f;  // Nivel actual de contaminación
    public Image fillBarImage;

    public Color colorAmarillo = Color.yellow;
    public Color colorNaranja = new Color(1f, 0.5f, 0f); // Color naranja
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
        // Inicializar la barra con 0 contaminación
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

        panelDialogo.SetActive(true);  // Activar el panel
        StartCoroutine(EscribirTexto(mensajesDisturbios[indiceMensajeActual]));


        // Esperar hasta que el mensaje haya sido completamente escrito
        while (estaEscribiendo)
        {
            yield return null;
        }

        // Mantener el panel visible por un tiempo antes de ocultarlo
        yield return new WaitForSeconds(duracionMensaje);

        panelDialogo.SetActive(false);

        audioSeguridad.Stop();

        yield return new WaitForSeconds(4f);

        ruidosDisturbios.Stop();


        if (radioAnimator != null)
        {
            radioAnimator.SetTrigger("IdleRadio"); // Asumiendo que tienes un estado 'idle' en tu animación
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

        // Actualizar la barra visualmente
        if (barraContaminacion != null)
        {
            barraContaminacion.value = nivelContaminacion;
            ActualizarColorBarra();
            loadingBar.Play();
        }

        // Puedes agregar lógica adicional si el nivel de contaminación llega al máximo (perder el juego, etc.)
        if (nivelContaminacion >= maxContaminacion)
        {


            Debug.Log("¡El búnker está completamente contaminado! Has perdido.");
            // Lógica para manejar la pérdida del juego
        }
    }


    private void ActualizarColorBarra()
    {
        float porcentajeContaminacion = nivelContaminacion / maxContaminacion;

        if (porcentajeContaminacion <= 0.34f)
        {
            fillBarImage.color = colorAmarillo;  // Hasta 33% amarillo
        }
        else if (porcentajeContaminacion <= 0.67f)
        {
            fillBarImage.color = colorNaranja;  // Entre 33% y 66% naranja
        }
        else
        {
            fillBarImage.color = colorRojo;  // Más del 66% rojo
        }
    }


    private void Perder()
    {
        CharactersManager charactersManager = FindObjectOfType<CharactersManager>();
        if (charactersManager != null)
        {
            charactersManager.DetenerPersonajes();  // Detener la aparición de personajes
        }


        panelPerdiste.gameObject.SetActive(true);
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
