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
    public string mensajeDisturbios = "Un infectado generó disturbios, tené mas cuidado con quien dejas entrar.";
    public float velocidadEscritura = 0.05f;
    public float duracionMensaje = 3f;

    private bool estaEscribiendo = false;

    public Slider barraContaminacion;
    public float maxContaminacion = 5f;  // Máximo nivel de contaminación
    private float nivelContaminacion = 0f;  // Nivel actual de contaminación


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

        ruidosDisturbios.Play();
        audioSeguridad.Play();

        panelDialogo.SetActive(true);  // Activar el panel
        StartCoroutine(EscribirTexto(mensajeDisturbios));  // Escribir el texto gradualmente


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
        }

        // Puedes agregar lógica adicional si el nivel de contaminación llega al máximo (perder el juego, etc.)
        if (nivelContaminacion >= maxContaminacion)
        {
            Debug.Log("¡El búnker está completamente contaminado! Has perdido.");
            // Lógica para manejar la pérdida del juego
        }
    }
}
