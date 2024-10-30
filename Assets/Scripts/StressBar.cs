using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        yield return new WaitForSeconds(2f);

        mate.SetTrigger("mateIdle");
    }


    private void ActualizarColorBarra()
    {
        float porcentajeEstres = nivelEstres / maxEstres;

        if (porcentajeEstres <= 0.2f)
        {
            fillBarImage.color = colorVerdeClaro;  // Hasta 20%
        }
        else if (porcentajeEstres <= 0.4f)
        {
            fillBarImage.color = colorVerdeOscuro; // 20% - 40%
        }
        else if (porcentajeEstres <= 0.6f)
        {
            fillBarImage.color = colorAmarillo;    // 40% - 60%
        }
        else if (porcentajeEstres <= 0.8f)
        {
            fillBarImage.color = colorNaranja;     // 60% - 80%
        }
        else
        {
            fillBarImage.color = colorRojo;        // Más del 80%
        }
    }



    public void PerderJuego()
    {
        CharactersManager charactersManager = FindObjectOfType<CharactersManager>();
        if (charactersManager != null)
        {
            charactersManager.DetenerPersonajes();  // Detener la aparición de personajes
        }

        // Mostrar el panel de perder o cualquier otra lógica
        panelPerdiste.SetActive(true);
    }

}
