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
    public float duracionMensaje = 3f;

    private bool estaEscribiendo = false;

    public Slider barraContaminacion;
    public float maxContaminacion = 5f;  // Máximo nivel de contaminación
    private float nivelContaminacion = 0f;  // Nivel actual de contaminación
    public Image fillBarImage;

    public Color colorAmarillo = Color.yellow;
    public Color colorNaranja = new Color(1f, 0.5f, 0f); // Color naranja
    public Color colorRojo = Color.red;



    public Animator radioAnimator;

    private int indiceMensajeActual = 0;

 public s_GameManager gameManager;
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

        if (porcentajeContaminacion <= 0.5f)
        {
            fillBarImage.color = colorAmarillo;  // Hasta 50% amarillo
        }
        else if (porcentajeContaminacion <= 0.8f)
        {
            fillBarImage.color = colorNaranja;  // Entre 50% y 80% naranja
        }
        else
        {
            fillBarImage.color = colorRojo;  // Más del 80% rojo
        }
    }
}
