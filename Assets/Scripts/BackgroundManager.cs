using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public float velocidadMovimiento = 0.5f; 
    private Vector3 startPosition;

    public GameObject capaNivel1;
    public GameObject capaNivel2;
    public GameObject capaCorteDeLuz; // Capa para el efecto de corte de luz
    public float tiempoParpadeo = 0.2f; // Tiempo de parpadeo en segundos
    public float duracionCorte = 3.0f; // Duración total del corte de luz
    public float frecuenciaCorte = 5.0f; // Tiempo entre cortes de luz en segundos

    void Start()
    {
        startPosition = transform.position;

        MostrarCapaPorNivel();

        // Iniciar la corrutina si es nivel 2
       
      /*   if (GameData.NivelActual == 2)
        {
            StartCoroutine(SimularCorteDeLuz());
        } */
    }

    void Update()
    {
        float nuevoPosX = Mathf.Repeat(Time.time * velocidadMovimiento, 30f); 
        transform.position = startPosition + Vector3.right * nuevoPosX;
    }

    void MostrarCapaPorNivel()
    {
        if (GameData.NivelActual == 1)
        {
            capaNivel1.SetActive(true);
            capaNivel2.SetActive(false);
            capaCorteDeLuz.SetActive(false); // Asegúrate de desactivar la capa de corte de luz
        }
        else if (GameData.NivelActual == 2)
        {
            capaNivel1.SetActive(false);
            capaNivel2.SetActive(true);
            capaCorteDeLuz.SetActive(false); // Asegúrate de desactivar la capa de corte de luz
        }
        else
        {
            capaNivel1.SetActive(false);
            capaNivel2.SetActive(false);
            capaCorteDeLuz.SetActive(false);
        }
    }

    IEnumerator SimularCorteDeLuz()
    {
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionCorte)
        {
           

              for (int i = 0; i < 3; i++)
        {
             // Activar la capa de corte de luz
            capaCorteDeLuz.SetActive(true);

            // Esperar un tiempo antes de desactivar
            yield return new WaitForSeconds(tiempoParpadeo);

            // Desactivar la capa de corte de luz
            capaCorteDeLuz.SetActive(false);

            // Esperar un tiempo antes de volver a activar
            yield return new WaitForSeconds(tiempoParpadeo);
        }

            

            // Sumar el tiempo transcurrido
            tiempoTranscurrido += tiempoParpadeo * 2; // 2 porque activamos y desactivamos
            
            // Esperar el tiempo de frecuencia entre cortes
            yield return new WaitForSeconds(frecuenciaCorte);
        }

        // Asegúrate de desactivar la capa de corte de luz al final
        capaCorteDeLuz.SetActive(false);
    }
}