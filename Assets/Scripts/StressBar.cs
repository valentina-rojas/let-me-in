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

    public Color colorVerde = Color.green;
    public Color colorAmarillo = Color.yellow;
    public Color colorRojo = Color.red;

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


     private void PerderJuego()
    {
     
    
        if (panelPerdiste != null)
        {
            panelPerdiste.SetActive(true);
        }

    }
}
