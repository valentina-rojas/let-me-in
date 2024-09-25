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

 
    public void ActivarDisturbios()
    {
        StartCoroutine(ActivarDisturbiosCoroutine());
    }

    public IEnumerator ActivarDisturbiosCoroutine()
    {

        yield return new WaitForSeconds(15f);

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
}
