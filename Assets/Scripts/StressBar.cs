using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    public Slider barraEstres;  // Referencia al Slider de la barra de estrés
    public float maxEstres = 5f;  // Máximo nivel de estrés
    private float nivelEstres = 0f;  // Nivel actual de estrés
    public Image fillBarImage;  // Imagen de la barra para cambiar color

    public Color colorVerde = Color.green;
    public Color colorAmarillo = Color.yellow;
    public Color colorRojo = Color.red;

  void Start()
    {
        // Inicializar la barra con 0 estrés
        if (barraEstres != null)
        {
            barraEstres.maxValue = maxEstres;
            barraEstres.value = nivelEstres;
        }
    }

    // Llamar esta función cuando se va un personaje
    public void ActualizarEstres(float cantidad)
    {
        nivelEstres += cantidad;

        // Asegurarse de no exceder el nivel máximo de estrés
        if (nivelEstres > maxEstres)
        {
            nivelEstres = maxEstres;
        }

        // Actualizar la barra visualmente
        if (barraEstres != null)
        {
            barraEstres.value = nivelEstres;
            ActualizarColorBarra();
        }

        // Puedes agregar lógica adicional si el nivel de estrés llega al máximo (pérdida, eventos especiales, etc.)
        if (nivelEstres >= maxEstres)
        {
            Debug.Log("¡El nivel de estrés ha alcanzado su límite!");
            // Lógica para manejar qué sucede cuando el estrés está al máximo
        }
    }

    private void ActualizarColorBarra()
    {
        float porcentajeEstres = nivelEstres / maxEstres;

        if (porcentajeEstres <= 0.5f)
        {
            fillBarImage.color = colorVerde;  // Hasta 50% verde
        }
        else if (porcentajeEstres <= 0.8f)
        {
            fillBarImage.color = colorAmarillo;  // Entre 50% y 80% amarillo
        }
        else
        {
            fillBarImage.color = colorRojo;  // Más del 80% rojo
        }
    }
}
